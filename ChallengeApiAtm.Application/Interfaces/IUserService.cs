using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;

namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de gestión de usuarios
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registra un nuevo usuario en el sistema con tarjeta personalizada
    /// </summary>
    /// <param name="request">Datos del nuevo usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del usuario registrado</returns>
    Task<RegisterResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}