using System.Security.Claims;

namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de generación y validación de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Genera un token JWT
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <returns>Token JWT</returns>
    string GenerateToken(Guid userId, string cardNumber, string accountNumber);

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Claims principal si es válido</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Obtiene el tiempo de expiración del token en segundos
    /// </summary>
    /// <returns>Tiempo de expiración en segundos</returns>
    int GetTokenExpirationInSeconds();

    /// <summary>
    /// Obtiene el número de tarjeta del token
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>Número de tarjeta si es válido</returns>
    string? GetCardNumberFromToken(string token);

    /// <summary>
    /// Obtiene el ID del usuario del token
    /// </summary>
    /// <param name="token">Token a validar</param>
    /// <returns>ID del usuario si es válido</returns>
    Guid? GetUserIdFromToken(string token);
}