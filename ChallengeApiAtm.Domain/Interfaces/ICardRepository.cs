using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.Domain.Interfaces;

/// <summary>
/// Contrato para el repositorio de tarjetas
/// </summary>
public interface ICardRepository
{
    /// <summary>
    /// Obtiene una tarjeta por su ID
    /// </summary>
    /// <param name="id">ID de la tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta encontrada o null</returns>
    Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una tarjeta por su número
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta encontrada o null</returns>
    Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una tarjeta por su número incluyendo datos del usuario y cuenta
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta con datos relacionados o null</returns>
    Task<Card?> GetByCardNumberWithRelatedDataAsync(string cardNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una tarjeta por su número incluyendo datos del usuario y cuenta (alias para compatibilidad)
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta con datos relacionados o null</returns>
    Task<Card?> GetByCardNumberWithDetailsAsync(string cardNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las tarjetas de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de tarjetas del usuario</returns>
    Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva tarjeta
    /// </summary>
    /// <param name="card">Tarjeta a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Card card, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una tarjeta existente
    /// </summary>
    /// <param name="card">Tarjeta a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Card card, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una tarjeta
    /// </summary>
    /// <param name="card">Tarjeta a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Card card, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una tarjeta con el número especificado
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    Task<bool> ExistsByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default);
} 