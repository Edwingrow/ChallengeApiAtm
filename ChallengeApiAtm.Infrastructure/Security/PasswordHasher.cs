using ChallengeApiAtm.Application.Interfaces;
using BCrypt.Net;

namespace ChallengeApiAtm.Infrastructure.Security;

/// <summary>
/// Implementación del servicio de hash de contraseñas usando BCrypt
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; 

    /// <summary>
    /// Hashea un PIN usando BCrypt
    /// </summary>
    /// <param name="pin">PIN a hashear</param>
    /// <returns>PIN hasheado</returns>
    public string HashPin(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin))
            throw new ArgumentException("El PIN no puede estar vacío", nameof(pin));

        return BCrypt.Net.BCrypt.HashPassword(pin, WorkFactor);
    }

    /// <summary>
    /// Verifica si un PIN coincide con su hash
    /// </summary>
    /// <param name="pin">PIN a verificar</param>
    /// <param name="hashedPin">PIN hasheado</param>
    /// <returns>True si el PIN es válido, false en caso contrario</returns>
    public bool VerifyPin(string pin, string hashedPin)
    {
        if (string.IsNullOrWhiteSpace(pin))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPin))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(pin, hashedPin);
        }
        catch
        {
           
            return false;
        }
    }
} 