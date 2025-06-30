namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta de registro de usuario
/// </summary>
public sealed class RegisterResponse
{
    /// <summary>
    /// ID del usuario
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Número de cuenta
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;
    /// <summary>
    /// Número de tarjeta
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    /// <summary>
    /// Saldo inicial de la cuenta
    /// </summary>
    public decimal InitialBalance { get; set; }
    /// <summary>
    /// Fecha de expiración de la tarjeta
    /// </summary>
    public string ExpiryDate { get; set; } = string.Empty;
    /// <summary>
    /// Fecha de registro del usuario
    /// </summary>
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Mensaje de respuesta
    /// </summary>
    public string Message { get; set; } = "Usuario registrado exitosamente";
}