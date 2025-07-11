using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Application.Services;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Exceptions;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.UnitTests.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChallengeApiAtm.UnitTests.Application.Services;

/// <summary>
/// Tests for the AtmService class
/// </summary>
public class AtmServiceTests
{
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IUserContextService> _userContextServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly AtmService _atmService;

    public AtmServiceTests()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _userContextServiceMock = new Mock<IUserContextService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _atmService = new AtmService(
            _cardRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _userContextServiceMock.Object,
            _passwordHasherMock.Object);
    }

    [Fact]
    public async Task WithdrawAsync_ValidRequest_ShouldReturnWithdrawResponse()
    {

        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithInitialBalance(1000);

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new WithdrawRequest
        {
            CardNumber = card.CardNumber,
            Amount = 100
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _accountRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Account>(), default))
            .Returns(Task.CompletedTask);

        _transactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);

        
        var result = await _atmService.WithdrawAsync(request);


        result.Should().NotBeNull();
        result.WithdrawnAmount.Should().Be(100);
        result.PreviousBalance.Should().Be(1000);
        result.NewBalance.Should().Be(900);
        result.AccountNumber.Should().Be(account.AccountNumber);

        
        _accountRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Account>(), default), Times.Once);
        _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>(), default), Times.Once);
    }

    [Fact]
    public async Task WithdrawAsync_UnauthorizedCard_ShouldThrowUnauthorizedAccessException()
    {

        var request = new WithdrawRequest
        {
            CardNumber = "1234567890123456",
            Amount = 100
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(false);


        var action = async () => await _atmService.WithdrawAsync(request);
        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("No tiene permisos para acceder a esta tarjeta");
    }

    [Fact]
    public async Task GetBalanceAsync_InvalidCard_ShouldThrowInvalidOperationException()
    {
        var request = new BalanceRequest
        {
            CardNumber = "1234567890123456"
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync((Card?)null);

        var action = async () => await _atmService.GetBalanceAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarjeta no válida o inactiva");
    }

    [Fact]
    public async Task IsCardValidAsync_ValidCard_ShouldReturnTrue()
    {
        var cardNumber = "1234567890123456";

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberAsync(cardNumber, default))
            .ReturnsAsync(new CardBuilder().Build());

        var result = await _atmService.IsCardValidAsync(cardNumber);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetBalanceAsync_UnauthorizedCard_ShouldThrowUnauthorizedAccessException()
    {

        var request = new BalanceRequest
        {
            CardNumber = "1234567890123456"
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(false);


        var action = async () => await _atmService.GetBalanceAsync(request);
        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("No tiene permisos para acceder a esta tarjeta");
    }

    [Fact]
    public async Task GetBalanceAsync_ValidRequest_ShouldReturnBalanceResponse()
    {

        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithInitialBalance(1000);

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new BalanceRequest
        {
            CardNumber = card.CardNumber
        };

        var lastTransaction = new TransactionBuilder()
            .WithAccountId(account.Id)
            .WithAmount(100)
            .Build();

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _transactionRepositoryMock
            .Setup(x => x.GetLastWithdrawalByAccountIdAsync(account.Id, default))
            .ReturnsAsync(lastTransaction);

        _transactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);

        
        var result = await _atmService.GetBalanceAsync(request);

        
        result.Should().NotBeNull();
        result.CurrentBalance.Should().Be(1000);
        result.UserName.Should().Be(user.FullName);
        result.AccountNumber.Should().Be(account.AccountNumber);
        result.LastWithdrawalDate.Should().Be(lastTransaction.CreatedAt); 
        result.ConsultationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetOperationsAsync_ValidRequest_ShouldReturnOperationsResponse()
    {
        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithInitialBalance(1000);

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new OperationsRequest
        {
            CardNumber = card.CardNumber
        };

        var transactions = new List<Transaction>
    {
        new TransactionBuilder()
            .WithAccountId(account.Id)
            .WithAmount(100)
            .Build(),
        new TransactionBuilder()
            .WithAccountId(account.Id)
            .WithAmount(50)
            .Build()
    };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _transactionRepositoryMock
            .Setup(x => x.GetPaginatedByAccountIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                default))
            .ReturnsAsync((transactions, transactions.Count));

        var result = await _atmService.GetOperationsAsync(request, pageNumber: 1, pageSize: 10);


        result.Should().NotBeNull();
        result.Transactions.Should().HaveCount(2);
        result.AccountInfo.Should().NotBeNull();
        result.AccountInfo.AccountNumber.Should().Be(account.AccountNumber);
        result.AccountInfo.AccountHolderName.Should().Be(user.FullName);
        result.AccountInfo.CurrentBalance.Should().Be(account.Balance);


        result.Pagination.Should().NotBeNull();
        result.Pagination.CurrentPage.Should().Be(1);
        result.Pagination.PageSize.Should().Be(10);
        result.Pagination.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task GetOperationsAsync_UnauthorizedCard_ShouldThrowUnauthorizedAccessException()
    {
        
        var request = new OperationsRequest
        {
            CardNumber = "1234567890123456"
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(false);

       
        var action = async () => await _atmService.GetOperationsAsync(request, 1, 10);
        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Tarjeta no válida o incorrecta");
    }

    [Fact]
    public async Task WithdrawAsync_InvalidCard_ShouldThrowInvalidOperationException()
    {
        var request = new WithdrawRequest
        {
            CardNumber = "1234567890123456",
            Amount = 100
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync((Card?)null);

       
        var action = async () => await _atmService.WithdrawAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarjeta no válida o inactiva");
    }

    [Fact]
    public async Task WithdrawAsync_InsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        
        var scenario = new TestScenarioBuilder()
            .WithCardNumber("1234567890123456")
            .WithInitialBalance(50); 

        var (user, account, card) = scenario.BuildCompleteScenario();

        var request = new WithdrawRequest
        {
            CardNumber = card.CardNumber,
            Amount = 100  
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync(card);

        _transactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Transaction>(), default))
            .Returns(Task.CompletedTask);

       
        var action = async () => await _atmService.WithdrawAsync(request);
        await action.Should().ThrowAsync<InsufficientFundsException>()
            .WithMessage($"Fondos insuficientes. Solicitado: {request.Amount:C}, Disponible: {account.Balance:C}");

        _transactionRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Transaction>(t => 
                t.Description.Contains("Retiro ATM - Rechazado: Fondos insuficientes")), default), 
            Times.Once);
    }

    [Fact]
    public async Task GetOperationsAsync_InvalidCard_ShouldThrowInvalidOperationException()
    {
        
        var request = new OperationsRequest
        {
            CardNumber = "1234567890123456"
        };

        _userContextServiceMock
            .Setup(x => x.ValidateCardOwnership(request.CardNumber))
            .Returns(true);

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberWithDetailsAsync(request.CardNumber, default))
            .ReturnsAsync((Card?)null);

       
        var action = async () => await _atmService.GetOperationsAsync(request, 1, 10);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Tarjeta no válida o inactiva");
    }

    [Fact]
    public async Task IsCardValidAsync_NonExistentCard_ShouldReturnFalse()
    {
        
        var cardNumber = "9999999999999999";

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberAsync(cardNumber, default))
            .ReturnsAsync((Card?)null);

        var result = await _atmService.IsCardValidAsync(cardNumber);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCardValidAsync_InactiveCard_ShouldReturnFalse()
    {
        
        var cardNumber = "1234567890123456";
        var inactiveCard = new CardBuilder().Build();
        
        
       typeof(Card).GetProperty("ExpiryDate")?.SetValue(inactiveCard, DateTime.UtcNow.AddDays(-1));

        _cardRepositoryMock
            .Setup(x => x.GetByCardNumberAsync(cardNumber, default))
            .ReturnsAsync(inactiveCard);

       
        var result = await _atmService.IsCardValidAsync(cardNumber);

       
        result.Should().BeFalse();
    }
}