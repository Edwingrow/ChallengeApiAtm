using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Interfaces;
using System.Globalization;

namespace ChallengeApiAtm.Application.Services;

/// <summary>
/// Servicio para gestión de usuarios (registro)
/// </summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly INumberGeneratorService _numberGeneratorService;

    public UserService(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ICardRepository cardRepository,
        IPasswordHasher passwordHasher,
        INumberGeneratorService numberGeneratorService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _numberGeneratorService = numberGeneratorService ?? throw new ArgumentNullException(nameof(numberGeneratorService));
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema con tarjeta personalizada
    /// </summary>
    /// <param name="request">Datos del nuevo usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del usuario registrado</returns>
    public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByDocumentNumberAsync(request.DocumentNumber, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Ya existe un usuario registrado con este número de documento");
        }

        var existingCard = await _cardRepository.ExistsByCardNumberAsync(request.CardNumber, cancellationToken);
        if (existingCard)
        {
            throw new InvalidOperationException("Ya existe una tarjeta con este número");
        }

        var expiryDate = ParseExpiryDate(request.ExpiryDate);
        if (expiryDate <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("La fecha de vencimiento debe ser mayor a la fecha actual");
        }

        var user = new User(request.FirstName, request.LastName, request.DocumentNumber);
        await _userRepository.AddAsync(user, cancellationToken);

        var accountNumber = await _numberGeneratorService.GenerateUniqueAccountNumberAsync(cancellationToken);
        var account = new Account(accountNumber, user.Id, request.InitialBalance);
        await _accountRepository.AddAsync(account, cancellationToken);

        var hashedPin = _passwordHasher.HashPin(request.Pin);
        var card = new Card(request.CardNumber, hashedPin, user.Id, account.Id);

        card.SetCustomExpiryDate(expiryDate);
        await _cardRepository.AddAsync(card, cancellationToken);

        return new RegisterResponse
        {
            UserId = user.Id,
            AccountNumber = accountNumber,
            CardNumber = MaskCardNumber(request.CardNumber),
            FirstName = user.FirstName,
            LastName = user.LastName,
            InitialBalance = request.InitialBalance,
            ExpiryDate = request.ExpiryDate,
            RegistrationDate = DateTime.UtcNow,
            Message = "Usuario registrado exitosamente"
        };
    }

    /// <summary>
    /// Enmascara el número de tarjeta mostrando solo los últimos 4 dígitos
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta completo</param>
    /// <returns>Número de tarjeta enmascarado</returns>
    private static string MaskCardNumber(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
            return "****";

        return "****-****-****-" + cardNumber[^4..];
    }

    /// <summary>
    /// Convierte la fecha de vencimiento a formato UTC
    /// </summary>
    /// <param name="expiryDate">Fecha de vencimiento en formato dd/MM/yyyy</param>
    /// <returns>Fecha de vencimiento en formato UTC</returns>
    private static DateTime ParseExpiryDate(string expiryDate)
    {
        if (!DateTime.TryParseExact($"01/{expiryDate}", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            throw new InvalidOperationException("Formato de fecha de vencimiento inválido");
        }

        // La tarjeta vence al final del mes especificado
        var expiryEndOfMonth = parsedDate.AddMonths(1).AddDays(-1);
        
        // Convertir a UTC para compatibilidad con PostgreSQL
        return DateTime.SpecifyKind(expiryEndOfMonth, DateTimeKind.Utc);
    }

}