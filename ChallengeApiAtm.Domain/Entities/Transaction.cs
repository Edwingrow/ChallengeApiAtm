using ChallengeApiAtm.Domain.Enums;

namespace ChallengeApiAtm.Domain.Entities;

public class Transaction
{
    /// <summary>
    /// Inicializa una nueva instancia de Transaction
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cardId">ID de la tarjeta</param>
    /// <param name="type">Tipo de transacción</param>
    /// <param name="amount">Monto de la transacción</param>
    /// <param name="description">Descripción de la transacción</param>
    /// <exception cref="ArgumentException">Cuando los parámetros son inválidos</exception>
    public Transaction(Guid accountId, Guid cardId, TransactionType type, decimal amount, string description)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("El ID de cuenta es requerido", nameof(accountId));

        if (cardId == Guid.Empty)
            throw new ArgumentException("El ID de tarjeta es requerido", nameof(cardId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción es requerida", nameof(description));

        if (type == TransactionType.Withdrawal && amount <= 0)
            throw new ArgumentException("Los retiros deben tener monto positivo", nameof(amount));

        Id = Guid.NewGuid();
        AccountId = accountId;
        CardId = cardId;
        Type = type;
        Amount = amount;
        Description = description.Trim();
        Status = TransactionStatus.Completed;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Inicializa una nueva instancia de Transaction
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cardId">ID de la tarjeta</param>
    /// <param name="type">Tipo de transacción</param>
    public Transaction(Guid accountId, Guid cardId, TransactionType type, decimal amount, string description, decimal? balanceAfterTransaction = null)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("El ID de cuenta es requerido", nameof(accountId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("La descripción es requerida", nameof(description));
            
        if (type == TransactionType.Withdrawal && amount <= 0)
            throw new ArgumentException("Los retiros deben tener monto positivo", nameof(amount));
        Id = Guid.NewGuid();
        AccountId = accountId;
        CardId = cardId;
        Type = type;
        Amount = amount;
        Description = description.Trim();
        Status = TransactionStatus.Completed;
        CreatedAt = DateTime.UtcNow;
        BalanceAfterTransaction = balanceAfterTransaction;
    }

    private Transaction() { }
    /// <summary>
    /// ID de la transacción
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// ID de la cuenta
    /// </summary>
    public Guid AccountId { get; private set; }
    /// <summary>
    /// ID de la tarjeta
    /// </summary>
    public Guid? CardId { get; private set; }
    /// <summary>
    /// Tipo de transacción
    /// </summary>
    public TransactionType Type { get; private set; }
    /// <summary>
    /// Monto de la transacción
    /// </summary>
    public decimal Amount { get; private set; }
    /// <summary>
    /// Descripción de la transacción
    /// </summary>
    public string Description { get; private set; } = string.Empty;
    /// <summary>
    /// Estado de la transacción
    /// </summary>
    public TransactionStatus Status { get; private set; }
    /// <summary>
    /// Fecha de creación de la transacción
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    /// <summary>
    /// Saldo después de la transacción
    /// </summary>
    public decimal? BalanceAfterTransaction { get; set; }
    /// <summary>
    /// Cuenta asociada a la transacción
    /// </summary>
    public virtual Account Account { get; private set; } = null!;
    /// <summary>
    /// Tarjeta asociada a la transacción
    /// </summary>
    public virtual Card? Card { get; private set; }

}