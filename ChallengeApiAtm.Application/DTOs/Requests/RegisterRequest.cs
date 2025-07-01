namespace ChallengeApiAtm.Application.DTOs.Requests;

/// <summary>
/// DTO para la solicitud de registro de usuario
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Número de documento del usuario
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;
    /// <summary>
    /// Número de tarjeta del usuario
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
    /// <summary>
    /// Fecha de expiración de la tarjeta
    /// </summary>
    public string ExpiryDate { get; set; } = string.Empty;
    /// <summary>
    /// PIN de la tarjeta
    /// </summary>
    public string Pin { get; set; } = string.Empty;
    /// <summary>
    /// Confirmación del PIN de la tarjeta
    /// </summary>
    public string ConfirmPin { get; set; } = string.Empty;
    /// <summary>
    /// Saldo inicial de la cuenta
    /// </summary>
    public decimal InitialBalance { get; set; } = 0;
}