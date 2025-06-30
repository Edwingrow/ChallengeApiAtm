namespace ChallengeApiAtm.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario
/// </summary>
public class User
{
    public User(string firstName, string lastName, string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre es requerido", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido es requerido", nameof(lastName));

        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException("El número de documento es requerido", nameof(documentNumber));

        Id = Guid.NewGuid();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DocumentNumber = documentNumber.Trim();
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    private User() { }
    /// <summary>
    /// ID del usuario
    /// </summary>
    public Guid Id { get; private set; }
    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;
    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public string LastName { get; private set; } = string.Empty;
    /// <summary>
    /// Número de documento del usuario
    /// </summary>
    public string DocumentNumber { get; private set; } = string.Empty;
    /// <summary>
    /// Indica si el usuario está activo
    /// </summary>
    public bool IsActive { get; private set; }
    /// <summary>
    /// Fecha de creación del usuario
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    /// <summary>
    /// Tarjetas asociadas al usuario
    /// </summary>
    public virtual ICollection<Card> Cards { get; private set; } = new List<Card>();
    /// <summary>
    /// Cuentas asociadas al usuario
    /// </summary>
    public virtual ICollection<Account> Accounts { get; private set; } = new List<Account>();
}