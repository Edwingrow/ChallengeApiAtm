namespace ChallengeApiAtm.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando se intenta usar una tarjeta bloqueada
/// </summary>
public sealed class CardBlockedException : DomainException
{
    /// <summary>
    /// Número de la tarjeta bloqueada
    /// </summary>
    public string CardNumber { get; }

    /// <summary>
    /// Inicializa una nueva instancia de CardBlockedException
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta bloqueada</param>
    public CardBlockedException(string cardNumber) 
        : base($"La tarjeta {cardNumber} está bloqueada por múltiples intentos fallidos")
    {
        CardNumber = cardNumber;
    }

} 