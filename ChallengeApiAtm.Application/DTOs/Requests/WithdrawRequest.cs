namespace ChallengeApiAtm.Application.DTOs.Requests;

/// <summary>
/// DTO para la solicitud de retiro en el ATM
/// </summary>
public sealed class WithdrawRequest
{
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Monto a retirar
    /// </summary>
    public decimal Amount { get; set; }
} 