namespace ChallengeApiAtm.Domain.Enums;

/// <summary>
/// Define los tipos de transacciones disponibles en el sistema ATM
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Retiro de dinero
    /// </summary>
    Withdrawal = 1,
    
    /// <summary>
    /// Consulta de saldo
    /// </summary>
    BalanceInquiry = 2,
    
} 