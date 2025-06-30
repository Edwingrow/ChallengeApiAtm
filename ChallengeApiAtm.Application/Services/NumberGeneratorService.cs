using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Domain.Interfaces;

namespace ChallengeApiAtm.Application.Services;

/// <summary>
/// Servicio para la generación de números únicos
/// </summary>
public sealed class NumberGeneratorService : INumberGeneratorService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    public NumberGeneratorService(
        IAccountRepository accountRepository,
        ICardRepository cardRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
    }
    /// <summary>
    /// Genera un número de cuenta único
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de cuenta generado</returns>
    public async Task<string> GenerateUniqueAccountNumberAsync(CancellationToken cancellationToken = default)
    {
        string accountNumber;
        do
        {
            var random = new Random();
            accountNumber = random.Next(1000000000, int.MaxValue).ToString();
        }
        while (await _accountRepository.ExistsByAccountNumberAsync(accountNumber, cancellationToken));

        return accountNumber;
    }
}