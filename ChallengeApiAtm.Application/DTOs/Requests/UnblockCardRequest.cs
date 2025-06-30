namespace ChallengeApiAtm.Application.DTOs.Requests;

/// <summary>
/// DTO para la solicitud de desbloqueo de tarjeta
/// </summary>
public sealed class UnblockCardRequest
{
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
    /// <summary>
    /// Número de documento de identidad del titular
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;
    /// <summary>
    /// Nuevo PIN para la tarjeta
    /// </summary>
    public string NewPin { get; set; } = string.Empty;
    /// <summary>
    /// Confirmación del nuevo PIN
    /// </summary>
    public string ConfirmNewPin { get; set; } = string.Empty;
}