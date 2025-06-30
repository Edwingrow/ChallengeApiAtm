using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApiAtm.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de tarjetas usando Entity Framework
/// </summary>
public sealed class CardRepository : ICardRepository
{
    private readonly AtmDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de tarjetas
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    public CardRepository(AtmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene una tarjeta por su ID
    /// </summary>
    /// <param name="id">ID de la tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta encontrada o null</returns>
    public async Task<Card?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtiene una tarjeta por su número
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta encontrada o null</returns>
    public async Task<Card?> GetByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
    }

    /// <summary>
    /// Obtiene una tarjeta por su número incluyendo datos del usuario y cuenta
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta con datos relacionados o null</returns>
    public async Task<Card?> GetByCardNumberWithRelatedDataAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Include(c => c.User)
            .Include(c => c.Account)
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber, cancellationToken);
    }

    /// <summary>
    /// Obtiene una tarjeta por su número incluyendo datos del usuario y cuenta (alias para compatibilidad)
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta con datos relacionados o null</returns>
    public async Task<Card?> GetByCardNumberWithDetailsAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await GetByCardNumberWithRelatedDataAsync(cardNumber, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las tarjetas de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de tarjetas del usuario</returns>
    public async Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva tarjeta
    /// </summary>
    /// <param name="card">Tarjeta a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta agregada</returns>
    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        await _context.Cards.AddAsync(card, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza una tarjeta existente
    /// </summary>
    /// <param name="card">Tarjeta a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta actualizada</returns>
    public async Task UpdateAsync(Card card, CancellationToken cancellationToken = default)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina una tarjeta
    /// </summary>
    /// <param name="card">Tarjeta a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tarjeta eliminada</returns>
    public async Task DeleteAsync(Card card, CancellationToken cancellationToken = default)
    {
        _context.Cards.Remove(card);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe una tarjeta con el número especificado
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    public async Task<bool> ExistsByCardNumberAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Cards
            .AnyAsync(c => c.CardNumber == cardNumber, cancellationToken);
    }
} 