namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de hash de PINs
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash del PIN
    /// </summary>
    /// <param name="pin">PIN a hashear</param>
    /// <returns>Hash del PIN</returns>
    string HashPin(string pin);
    /// <summary>
    /// Verifica si un PIN coincide con su hash
    /// </summary>
    /// <param name="pin">PIN a verificar</param>
    /// <param name="hashedPin">Hash del PIN</param>
    /// <returns>True si el PIN coincide con el hash</returns>
    bool VerifyPin(string pin, string hashedPin);
} 