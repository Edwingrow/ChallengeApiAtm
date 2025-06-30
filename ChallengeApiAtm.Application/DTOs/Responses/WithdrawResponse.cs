namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de retiro en el ATM
/// </summary>
public sealed class WithdrawResponse
{
    /// <summary>
    /// ID de la transacción generada
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// Monto retirado
    /// </summary>
    public decimal WithdrawnAmount { get; set; }

    /// <summary>
    /// Saldo anterior antes del retiro
    /// </summary>
    public decimal PreviousBalance { get; set; }

    /// <summary>
    /// Nuevo saldo después del retiro
    /// </summary>
    public decimal NewBalance { get; set; }

    /// <summary>
    /// Fecha y hora de la operación
    /// </summary>
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Número de cuenta donde se realizó el retiro
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;
} 