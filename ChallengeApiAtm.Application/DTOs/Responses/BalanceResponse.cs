namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de consulta de saldo en el ATM
/// </summary>
public sealed class BalanceResponse
{
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Número de cuenta
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Saldo actual de la cuenta
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Fecha de la última extracción realizada
    /// </summary>
    public DateTime? LastWithdrawalDate { get; set; }

    /// <summary>
    /// Fecha de consulta
    /// </summary>
    public DateTime ConsultationDate { get; set; } = DateTime.UtcNow;
} 