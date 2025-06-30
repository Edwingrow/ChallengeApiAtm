namespace ChallengeApiAtm.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando se introduce un PIN inválido
/// </summary>
public sealed class InvalidPinException : DomainException
{   
    
    /// <summary>
    /// Número de intentos restantes antes del bloqueo
    /// </summary>
    public int RemainingAttempts { get; }
    
    /// <summary>
    /// Inicializa una nueva instancia de InvalidPinException
    /// </summary>
    /// <param name="remainingAttempts">Número de intentos restantes</param>
    public InvalidPinException(int remainingAttempts) 
        : base($"PIN inválido. Intentos restantes: {remainingAttempts}")
    {
        RemainingAttempts = remainingAttempts;
    }

} 