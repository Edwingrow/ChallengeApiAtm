namespace ChallengeApiAtm.Domain.Exceptions;

/// <summary>
/// Excepción base para todas las excepciones del dominio
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia de DomainException
    /// </summary>
    /// <param name="message">Mensaje de la excepción</param>
    protected DomainException(string message) : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de DomainException con excepción interna
    /// </summary>
    /// <param name="message">Mensaje de la excepción</param>
    /// <param name="innerException">Excepción interna</param>
    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 