using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Interfaces;
using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Exceptions;
using ChallengeApiAtm.Domain.Interfaces;

namespace ChallengeApiAtm.Application.Services;

/// <summary>
/// Servicio para la autenticación y gestión de tarjetas
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly ICardRepository _cardRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        ICardRepository cardRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService
        )
    {
        _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }
    /// <summary>
    /// Autentica un usuario con tarjeta y PIN
    /// </summary>
    /// <param name="request">Datos de login</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta con token JWT si es exitoso</returns>
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var card = await _cardRepository.GetByCardNumberWithDetailsAsync(request.CardNumber, cancellationToken);
        if (card == null)
        {
            throw new InvalidPinException(0);
        }
        var isPinValid = _passwordHasher.VerifyPin(request.Pin, card.HashedPin);

        try
        {
            card.ValidatePin(isPinValid);
        }
        catch (CardBlockedException)
        {
            await _cardRepository.UpdateAsync(card, cancellationToken);
            throw; 
        }
        catch (InvalidPinException)
        {
            await _cardRepository.UpdateAsync(card, cancellationToken);
            throw; 
        }

        await _cardRepository.UpdateAsync(card, cancellationToken);

        var token = _jwtTokenService.GenerateToken(
            card.UserId,
            card.CardNumber,
            card.Account.AccountNumber);

        var maskedCardNumber = MaskCardNumber(card.CardNumber);

        return new LoginResponse
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresIn = _jwtTokenService.GetTokenExpirationInSeconds(),
            UserInfo = new UserInfoDto
            {
                FullName = card.User.FullName,
                AccountNumber = card.Account.AccountNumber,
                CardNumber = maskedCardNumber
            }
        };
    }

    /// <summary>
    /// Obtiene información del usuario desde un token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Información del usuario o null si el token es inválido</returns>
    public async Task<UserInfoDto?> GetUserInfoFromTokenAsync(string token)
    {
        try
        {
            var cardNumber = _jwtTokenService.GetCardNumberFromToken(token);
            if (string.IsNullOrEmpty(cardNumber))
                return null;

            var card = await _cardRepository.GetByCardNumberWithDetailsAsync(cardNumber);
            if (card == null)
                return null;

            var maskedCardNumber = MaskCardNumber(card.CardNumber);

            return new UserInfoDto
            {
                FullName = card.User.FullName,
                AccountNumber = card.Account.AccountNumber,
                CardNumber = maskedCardNumber
            };
        }
        catch
        {
            return null;
        }
    }
    /// <summary>
    /// Desbloquea una tarjeta
    /// </summary>
    /// <param name="request">Solicitud de desbloqueo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta de desbloqueo</returns>
    public async Task<UnblockCardResponse> UnblockCardAsync(UnblockCardRequest request, CancellationToken cancellationToken = default)
    {
        var card = await _cardRepository.GetByCardNumberWithDetailsAsync(request.CardNumber, cancellationToken);
        if (card == null)
        {
            throw new InvalidOperationException("Tarjeta no encontrada");
        }

        if (card.User.DocumentNumber != request.DocumentNumber)
        {
            throw new InvalidOperationException("El número de documento no coincide con el titular de la tarjeta");
        }
        
        if (card.Status != CardStatus.Blocked)
        {
            throw new InvalidOperationException("La tarjeta no está bloqueada");
        }
        
        var hashedNewPin = _passwordHasher.HashPin(request.NewPin);
        card.UnblockWithNewPin(hashedNewPin);
        await _cardRepository.UpdateAsync(card, cancellationToken);

        return new UnblockCardResponse
        {
            CardNumber = MaskCardNumber(card.CardNumber),
            CardHolderName = card.User.FullName,
            CardStatus = "Activa",
            UnblockDate = DateTime.UtcNow,
            Message = "Tarjeta desbloqueada exitosamente"
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
}