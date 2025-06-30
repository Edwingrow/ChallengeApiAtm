using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApiAtm.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de transacciones usando Entity Framework
/// </summary>
public sealed class TransactionRepository : ITransactionRepository
{
    private readonly AtmDbContext _context;

    public TransactionRepository(AtmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene una transacción por su ID
    /// </summary>
    /// <param name="id">ID de la transacción</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción encontrada o null</returns>
    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Card)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtiene el historial de transacciones de una cuenta con paginación
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="pageNumber">Número de página (base 1)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de transacciones</returns>
    public async Task<(IEnumerable<Transaction> transactions, int totalCount)> GetByAccountIdPagedAsync(
        Guid accountId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var transactions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(t => t.Account)
            .Include(t => t.Card)
            .ToListAsync(cancellationToken);

        return (transactions, totalCount);
    }

    /// <summary>
    /// Obtiene el historial de transacciones de una cuenta con paginación (alias para compatibilidad)
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="pageNumber">Número de página (base 1)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de transacciones</returns>
    public async Task<(IEnumerable<Transaction> transactions, int totalCount)> GetPaginatedByAccountIdAsync(
        Guid accountId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await GetByAccountIdPagedAsync(accountId, pageNumber, pageSize, cancellationToken);
    }

    /// <summary>
    /// Obtiene la última transacción de retiro de una cuenta
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Última transacción de retiro o null</returns>
    public async Task<Transaction?> GetLastWithdrawalByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Type == TransactionType.Withdrawal)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    

    /// <summary>
    /// Obtiene transacciones por tipo en un rango de fechas
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="transactionType">Tipo de transacción</param>
    /// <param name="fromDate">Fecha inicial</param>
    /// <param name="toDate">Fecha final</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de transacciones</returns>
    public async Task<IEnumerable<Transaction>> GetByAccountAndTypeAsync(
        Guid accountId, 
        TransactionType transactionType, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId 
                     && t.Type == transactionType 
                     && t.CreatedAt >= fromDate 
                     && t.CreatedAt <= toDate)
            .OrderByDescending(t => t.CreatedAt)
            .Include(t => t.Account)
            .Include(t => t.Card)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva transacción
    /// </summary>
    /// <param name="transaction">Transacción a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción agregada</returns>
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza una transacción existente
    /// </summary>
    /// <param name="transaction">Transacción a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción actualizada</returns>
    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina una transacción
    /// </summary>
    /// <param name="transaction">Transacción a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción eliminada</returns>
    public async Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el total de transacciones de una cuenta
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número total de transacciones</returns>
    public async Task<int> GetTransactionCountByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .CountAsync(t => t.AccountId == accountId, cancellationToken);
    }
} 