namespace ChallengeApiAtm.Domain.Enums;

/// <summary>
/// Define los posibles estados de una transacción
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Transacción completada exitosamente
    /// </summary>
    Completed = 1,
    
    /// <summary>
    /// Transacción falló
    /// </summary>
    Failed = 2,
} 