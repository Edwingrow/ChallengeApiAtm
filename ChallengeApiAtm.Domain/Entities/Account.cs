using ChallengeApiAtm.Domain.Exceptions;

namespace ChallengeApiAtm.Domain.Entities;

/// <summary>
/// Representa una cuenta bancaria en el sistema ATM
/// </summary>
public class Account
{
    /// <summary>
    /// Inicializa una nueva instancia de Account
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="userId">ID del usuario propietario</param>
    /// <param name="initialBalance">Saldo inicial</param>
    /// <exception cref="ArgumentException">Cuando los parámetros son inválidos</exception>
    public Account(string accountNumber, Guid userId, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("El número de cuenta es requerido", nameof(accountNumber));

        if (userId == Guid.Empty)
            throw new ArgumentException("El ID de usuario es requerido", nameof(userId));

        if (initialBalance < 0)
            throw new ArgumentException("El saldo inicial no puede ser negativo", nameof(initialBalance));

        Id = Guid.NewGuid();
        AccountNumber = accountNumber.Trim();
        UserId = userId;
        Balance = initialBalance;
        CreatedAt = DateTime.UtcNow;
        LastWithdrawalDate = null;
        IsActive = true;
    }

    private Account() { }

    /// <summary>
    /// ID de la cuenta
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Número de cuenta
    /// </summary>
    public string AccountNumber { get; private set; } = string.Empty;

    /// <summary>
    /// ID del usuario propietario
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Saldo actual de la cuenta
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Fecha de creación de la cuenta
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Fecha de la última extracción
    /// </summary>
    public DateTime? LastWithdrawalDate { get; private set; }

    /// <summary>
    /// Indica si la cuenta está activa
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Usuario propietario de la cuenta
    /// </summary>
    public virtual User User { get; private set; } = null!;

    /// <summary>
    /// Transacciones asociadas a la cuenta
    /// </summary>
    public virtual ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

    /// <summary>
    /// Realiza un retiro de dinero de la cuenta
    /// </summary>
    /// <param name="amount">Monto a retirar</param>
    /// <returns>Nuevo saldo después del retiro</returns>
    /// <exception cref="InvalidOperationException">Si la cuenta está inactiva</exception>
    /// <exception cref="ArgumentException">Si el monto es menor o igual a cero</exception>
    /// <exception cref="InsufficientFundsException">Si el saldo es insuficiente</exception>
    public decimal Withdraw(decimal amount)
    {
        if (!IsActive)
            throw new InvalidOperationException($"La cuenta {AccountNumber} está inactiva");

        if (amount <= 0)
            throw new ArgumentException("El monto debe ser mayor a cero", nameof(amount));

        if (Balance < amount)
            throw new InsufficientFundsException(amount, Balance);

        Balance -= amount;
        LastWithdrawalDate = DateTime.UtcNow;

        return Balance;
    }

} 