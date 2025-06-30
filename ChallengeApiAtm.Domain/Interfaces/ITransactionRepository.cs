using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Enums;

namespace ChallengeApiAtm.Domain.Interfaces;

/// <summary>
/// Contrato para el repositorio de transacciones
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Obtiene una transacción por su ID
    /// </summary>
    /// <param name="id">ID de la transacción</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Transacción encontrada o null</returns>
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de transacciones de una cuenta con paginación
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="pageNumber">Número de página (base 1)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de transacciones</returns>
    Task<(IEnumerable<Transaction> transactions, int totalCount)> GetByAccountIdPagedAsync(
        Guid accountId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de transacciones de una cuenta con paginación (alias para compatibilidad)
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="pageNumber">Número de página (base 1)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de transacciones</returns>
    Task<(IEnumerable<Transaction> transactions, int totalCount)> GetPaginatedByAccountIdAsync(
        Guid accountId, 
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la última transacción de retiro de una cuenta
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Última transacción de retiro o null</returns>
    Task<Transaction?> GetLastWithdrawalByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Obtiene transacciones por tipo en un rango de fechas
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="transactionType">Tipo de transacción</param>
    /// <param name="fromDate">Fecha inicial</param>
    /// <param name="toDate">Fecha final</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de transacciones</returns>
    Task<IEnumerable<Transaction>> GetByAccountAndTypeAsync(
        Guid accountId, 
        TransactionType transactionType, 
        DateTime fromDate, 
        DateTime toDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva transacción
    /// </summary>
    /// <param name="transaction">Transacción a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una transacción existente
    /// </summary>
    /// <param name="transaction">Transacción a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una transacción
    /// </summary>
    /// <param name="transaction">Transacción a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el total de transacciones de una cuenta
    /// </summary>
    /// <param name="accountId">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número total de transacciones</returns>
    Task<int> GetTransactionCountByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
} 