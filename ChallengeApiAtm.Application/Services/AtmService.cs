using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Exceptions;
using ChallengeApiAtm.Domain.Interfaces;

namespace ChallengeApiAtm.Application.Services;

/// <summary>
/// Servicio de operaciones del ATM
/// </summary>
public sealed class AtmService : IAtmService
{
    private readonly ICardRepository _cardRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserContextService _userContextService;
    private readonly IPasswordHasher _passwordHasher;

    public AtmService(
        ICardRepository cardRepository,
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUserContextService userContextService,
        IPasswordHasher passwordHasher)
    {
        _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Consulta el saldo de una cuenta
    /// </summary>
    /// <param name="request">Datos de la consulta de saldo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del saldo y cuenta</returns>
    public async Task<BalanceResponse> GetBalanceAsync(BalanceRequest request, CancellationToken cancellationToken = default)
    {   
        if (!_userContextService.ValidateCardOwnership(request.CardNumber))
        {
            throw new UnauthorizedAccessException("No tiene permisos para acceder a esta tarjeta");
        }

        var cardInfo = await _cardRepository.GetByCardNumberWithDetailsAsync(request.CardNumber, cancellationToken);

        if (cardInfo == null || !cardInfo.IsActive)
        {
            throw new InvalidOperationException("Tarjeta no válida o inactiva");
        }

        var lastWithdrawal = await _transactionRepository.GetLastWithdrawalByAccountIdAsync(
            cardInfo.AccountId, cancellationToken);

        var balanceInquiry = new Transaction(
            accountId: cardInfo.AccountId,
            cardId: cardInfo.Id,
            type: TransactionType.BalanceInquiry,
            amount: 0,
            description: "Consulta de saldo"
            );

        await _transactionRepository.AddAsync(balanceInquiry, cancellationToken);

        return new BalanceResponse
        {
            UserName = cardInfo.User.FullName,
            AccountNumber = cardInfo.Account.AccountNumber,
            CurrentBalance = cardInfo.Account.Balance,
            LastWithdrawalDate = lastWithdrawal?.CreatedAt,
            ConsultationDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Realiza un retiro de dinero
    /// </summary>
    /// <param name="request">Datos del retiro</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del retiro</returns>
    public async Task<WithdrawResponse> WithdrawAsync(WithdrawRequest request, CancellationToken cancellationToken = default)
    {
        if (!_userContextService.ValidateCardOwnership(request.CardNumber))
        {
            throw new UnauthorizedAccessException("No tiene permisos para acceder a esta tarjeta");
        }

        var card = await _cardRepository.GetByCardNumberWithDetailsAsync(request.CardNumber, cancellationToken);

        if (card == null || !card.IsActive)
        {
            throw new InvalidOperationException("Tarjeta no válida o inactiva");
        }

        var account = card.Account;
        var previousBalance = account.Balance;

        try
        {
            account.Withdraw(request.Amount);

            var successTransaction = new Transaction(
                account.Id,
                card.Id,
                TransactionType.Withdrawal,
                request.Amount,
                $"Retiro ATM - Confirmado"
                );

            successTransaction.BalanceAfterTransaction = account.Balance;

            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _transactionRepository.AddAsync(successTransaction, cancellationToken);

            return new WithdrawResponse
            {
                TransactionId = successTransaction.Id,
                WithdrawnAmount = request.Amount,
                PreviousBalance = previousBalance,
                NewBalance = account.Balance,
                TransactionDate = successTransaction.CreatedAt,
                AccountNumber = account.AccountNumber,
            };
        }
        catch (InsufficientFundsException)
        {
            var failedTransaction = new Transaction(
                account.Id,
                card.Id,
                TransactionType.Withdrawal,
                request.Amount,
                $"Retiro ATM - Rechazado: Fondos insuficientes");

            typeof(Transaction).GetProperty("Status")?.SetValue(failedTransaction, TransactionStatus.Failed);
            failedTransaction.BalanceAfterTransaction = previousBalance;

            await _transactionRepository.AddAsync(failedTransaction, cancellationToken);

            throw;
        }
    }

    /// <summary>
    /// Obtiene el historial de operaciones de una cuenta
    /// </summary>
    /// <param name="request">Datos de la consulta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Historial de operaciones</returns>
    public async Task<OperationsResponse> GetOperationsAsync(OperationsRequest request, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (!_userContextService.ValidateCardOwnership(request.CardNumber))
        {
            throw new UnauthorizedAccessException("Tarjeta no válida o incorrecta");
        }

        var card = await _cardRepository.GetByCardNumberWithDetailsAsync(request.CardNumber, cancellationToken);

        if (card == null || !card.IsActive)
        {
            throw new InvalidOperationException("Tarjeta no válida o inactiva");
        }

        var (transactions, totalCount) = await _transactionRepository.GetPaginatedByAccountIdAsync(
            card.AccountId,
            pageNumber,
            pageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var hasNextPage = pageNumber < totalPages;
        var hasPreviousPage = pageNumber > 1;

        return new OperationsResponse
        {
            Transactions = transactions.Select(MapToTransactionDto),
            Pagination = new PaginationInfoDto
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalCount,
                TotalPages = totalPages,
                HasNextPage = hasNextPage,
                HasPreviousPage = hasPreviousPage
            },
            AccountInfo = new AccountSummaryDto
            {
                AccountNumber = card.Account.AccountNumber,
                AccountHolderName = card.User.FullName,
                CurrentBalance = card.Account.Balance
            }
        };
    }

    public async Task<bool> IsCardValidAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        var card = await _cardRepository.GetByCardNumberAsync(cardNumber, cancellationToken);
        return card != null && card.IsActive;
    }

    private static TransactionDto MapToTransactionDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            Type = transaction.Type,
            TypeDescription = GetTransactionTypeDescription(transaction.Type),
            Amount = transaction.Amount,
            Description = transaction.Description,
            Date = transaction.CreatedAt,
            BalanceAfterTransaction = transaction.BalanceAfterTransaction,
            Status = transaction.Status
        };
    }

    private static string GetTransactionTypeDescription(TransactionType type)
    {
        return type switch
        {
            TransactionType.Withdrawal => "Retiro",
            TransactionType.BalanceInquiry => "Consulta de Saldo",
            _ => "Desconocido"
        };
    }
}