namespace ChallengeApiAtm.Application.DTOs.Requests;

/// <summary>
/// DTO para la solicitud de login en el ATM
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// PIN de la tarjeta
    /// </summary>
    public string Pin { get; set; } = string.Empty;
} 