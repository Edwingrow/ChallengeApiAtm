namespace ChallengeApiAtm.Domain.Enums;

/// <summary>
/// Define los posibles estados de una tarjeta en el sistema ATM
/// </summary>
public enum CardStatus
{
    /// <summary>
    /// Tarjeta activa y disponible para uso
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// Tarjeta bloqueada por intentos fallidos de PIN
    /// </summary>
    Blocked = 2,
    
    /// <summary>
    /// Tarjeta deshabilitada por el banco
    /// </summary>
    Disabled = 3,
    
    /// <summary>
    /// Tarjeta expirada
    /// </summary>
    Expired = 4
} 