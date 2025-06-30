using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Exceptions;

namespace ChallengeApiAtm.Domain.Entities;

/// <summary>
/// Entidad que representa una tarjeta de crédito
/// </summary>
public class Card
{
    private const int MaxFailedAttempts = 4;

    /// <summary>
    /// Inicializa una nueva instancia de Card
    /// </summary>
    /// <param name="cardNumber">Número de la tarjeta</param>
    /// <param name="hashedPin">PIN hasheado</param>
    /// <param name="userId">ID del usuario propietario</param>
    /// <param name="accountId">ID de la cuenta asociada</param>
    /// <exception cref="ArgumentException">Cuando los parámetros son inválidos</exception>
    public Card(string cardNumber, string hashedPin, Guid userId, Guid accountId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("El número de tarjeta es requerido", nameof(cardNumber));

        if (string.IsNullOrWhiteSpace(hashedPin))
            throw new ArgumentException("El PIN es requerido", nameof(hashedPin));

        if (userId == Guid.Empty)
            throw new ArgumentException("El ID de usuario es requerido", nameof(userId));

        if (accountId == Guid.Empty)
            throw new ArgumentException("El ID de cuenta es requerido", nameof(accountId));

        Id = Guid.NewGuid();
        CardNumber = cardNumber.Trim();
        HashedPin = hashedPin;
        UserId = userId;
        AccountId = accountId;
        Status = CardStatus.Active;
        FailedAttempts = 0;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = DateTime.UtcNow.AddYears(3);
    }

    private Card() { }
    /// <summary>
    /// ID de la tarjeta
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// Número de la tarjeta
    /// </summary>
    public string CardNumber { get; private set; } = string.Empty;
    /// <summary>
    /// Pin hasheado
    public string HashedPin { get; private set; } = string.Empty;
    /// <summary>
    /// ID del usuario propietario
    /// </summary>
    public Guid UserId { get; private set; }
    /// <summary>
    /// ID de la cuenta asociada
    /// </summary>
    public Guid AccountId { get; private set; }
    /// <summary>
    /// Estado de la tarjeta
    /// </summary>
    public CardStatus Status { get; private set; }
    /// <summary>
    /// Intentos fallidos de autenticación
    /// </summary>
    public int FailedAttempts { get; private set; }
    /// <summary>
    /// Fecha de creación de la tarjeta
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    /// <summary>
    /// Fecha de expiración de la tarjeta
    /// </summary>
    public DateTime ExpiryDate { get; private set; }
    /// <summary>
    /// Usuario propietario de la tarjeta
    /// </summary>
    public virtual User User { get; private set; } = null!;
    /// <summary>
    /// Cuenta asociada a la tarjeta
    /// </summary>
    public virtual Account Account { get; private set; } = null!;
    /// <summary>
    /// Indica si la tarjeta está activa
    /// </summary>
    public bool IsActive => Status == CardStatus.Active && ExpiryDate > DateTime.UtcNow;

    /// <summary>
    /// Valida el PIN de la tarjeta
    /// </summary>
    /// <param name="isPinValid">Indica si el PIN es válido</param>
    /// <returns>True si el PIN es válido</returns>
        /// <exception cref="CardBlockedException">Si la tarjeta está bloqueada</exception>
    /// <exception cref="InvalidPinException">Si el PIN es inválido</exception>
    public bool ValidatePin(bool isPinValid)
    {
        if (Status == CardStatus.Blocked)
            throw new CardBlockedException(CardNumber);

        if (!IsActive)
            throw new InvalidOperationException($"La tarjeta {CardNumber} no está activa");

        if (isPinValid)
        {
            FailedAttempts = 0;
            return true;
        }

        FailedAttempts++;
        var remainingAttempts = MaxFailedAttempts - FailedAttempts;

        if (remainingAttempts <= 0)
        {
            Status = CardStatus.Blocked;
            throw new CardBlockedException(CardNumber);
        }

        throw new InvalidPinException(remainingAttempts);
    }
    /// <summary>
    /// Desbloquea la tarjeta con un nuevo PIN
    /// </summary>
    /// <param name="newHashedPin">Nuevo PIN hasheado</param>
    /// <exception cref="ArgumentException">Si el nuevo PIN es inválido</exception>
    /// <exception cref="InvalidOperationException">Si la tarjeta no está bloqueada</exception>
    public void UnblockWithNewPin(string newHashedPin)
    {
        if (string.IsNullOrWhiteSpace(newHashedPin))
            throw new ArgumentException("El PIN hasheado es requerido", nameof(newHashedPin));

        if (Status != CardStatus.Blocked)
            throw new InvalidOperationException("Solo se pueden desbloquear tarjetas bloqueadas");

        Status = CardStatus.Active;
        HashedPin = newHashedPin;
        FailedAttempts = 0;
    }
    /// <summary>
    /// Establece una fecha de expiración personalizada
    /// </summary>
    /// <param name="customExpiryDate">Fecha de expiración personalizada</param>
    /// <exception cref="ArgumentException">Si la fecha de expiración es menor o igual a la fecha actual</exception>
    public void SetCustomExpiryDate(DateTime customExpiryDate)
    {
        if (customExpiryDate <= DateTime.UtcNow)
            throw new ArgumentException("La fecha de vencimiento debe ser mayor a la fecha actual", nameof(customExpiryDate));

        ExpiryDate = customExpiryDate;
    }
}