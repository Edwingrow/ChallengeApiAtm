namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de desbloqueo de tarjeta
/// </summary>
public sealed class UnblockCardResponse
{
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del titular de la tarjeta
    /// </summary>
    public string CardHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Estado de la tarjeta
    /// </summary>
    public string CardStatus { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de desbloqueo de la tarjeta
    /// </summary>
    public DateTime UnblockDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Mensaje de respuesta
    /// </summary>
    public string Message { get; set; } = "Tarjeta desbloqueada exitosamente";
}