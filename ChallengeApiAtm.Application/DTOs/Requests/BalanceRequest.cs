namespace ChallengeApiAtm.Application.DTOs.Requests;

/// <summary>
/// DTO para la solicitud de consulta de saldo
/// </summary>
public sealed class BalanceRequest
{
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
} 