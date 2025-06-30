namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Servicio para obtener información del usuario autenticado desde el contexto HTTP
/// </summary>
public interface IUserContextService
{
    /// <summary>
    /// Obtiene el número de tarjeta del usuario autenticado desde el token JWT
    /// </summary>
    /// <returns>Número de tarjeta si está autenticado, null si no</returns>
    string? GetAuthenticatedCardNumber();

    /// <summary>
    /// Obtiene el ID del usuario autenticado desde el token JWT
    /// </summary>
    /// <returns>ID del usuario si está autenticado, null si no</returns>
    Guid? GetAuthenticatedUserId();

    /// <summary>
    /// Valida que la tarjeta solicitada pertenece al usuario autenticado
    /// </summary>
    /// <param name="requestedCardNumber">Número de tarjeta solicitado</param>
    /// <returns>True si la tarjeta pertenece al usuario autenticado</returns>
    bool ValidateCardOwnership(string requestedCardNumber);
} 