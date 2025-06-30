namespace ChallengeApiAtm.Application.DTOs.Responses;

/// <summary>
/// DTO para la respuesta del login en el ATM
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// Token JWT generado para la sesión
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Tipo del token (siempre "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Tiempo de expiración del token en segundos
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Información del usuario autenticado
    /// </summary>
    public UserInfoDto UserInfo { get; set; } = null!;
}

/// <summary>
/// DTO con información básica del usuario
/// </summary>
public sealed class UserInfoDto
{
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Número de cuenta asociado a la tarjeta
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Número de tarjeta (parcialmente oculto por seguridad)
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
} 