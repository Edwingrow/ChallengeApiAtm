using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;

namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de autenticación del ATM
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario con tarjeta y PIN
    /// </summary>
    /// <param name="request">Datos de login</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta con token JWT si es exitoso</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene información del usuario desde un token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Información del usuario o null si el token es inválido</returns>
    /// 
    Task<UserInfoDto?> GetUserInfoFromTokenAsync(string token);

    /// <summary>
    /// Desbloquea una tarjeta
    /// </summary>
    /// <param name="request">Solicitud de desbloqueo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Respuesta de desbloqueo</returns>
    Task<UnblockCardResponse> UnblockCardAsync(UnblockCardRequest request, CancellationToken cancellationToken = default);
}