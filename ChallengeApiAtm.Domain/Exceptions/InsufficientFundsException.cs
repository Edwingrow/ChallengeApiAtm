namespace ChallengeApiAtm.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando se intenta retirar más dinero del disponible
/// </summary>
public sealed class InsufficientFundsException : DomainException
{
    /// <summary>
    /// Monto solicitado para el retiro
    /// </summary>
    public decimal RequestedAmount { get; }

    /// <summary>
    /// Saldo disponible en la cuenta
    /// </summary>
    public decimal AvailableBalance { get; }
    
    /// <summary>
    /// Inicializa una nueva instancia de InsufficientFundsException
    /// </summary>
    /// <param name="requestedAmount">Monto solicitado</param>
    /// <param name="availableBalance">Saldo disponible</param>
    public InsufficientFundsException(decimal requestedAmount, decimal availableBalance) 
        : base($"Fondos insuficientes. Solicitado: {requestedAmount:C}, Disponible: {availableBalance:C}")
    {
        RequestedAmount = requestedAmount;
        AvailableBalance = availableBalance;
    }

} 