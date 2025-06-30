using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApiAtm.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de cuentas usando Entity Framework
/// </summary>
public sealed class AccountRepository : IAccountRepository
{
    private readonly AtmDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de cuentas
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    public AccountRepository(AtmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene una cuenta por su ID
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta encontrada o null</returns>
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtiene una cuenta por su número
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta encontrada o null</returns>
    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }

    /// <summary>
    /// Obtiene una cuenta por su número incluyendo datos del usuario
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta con datos del usuario o null</returns>
    public async Task<Account?> GetByAccountNumberWithUserAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las cuentas de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de cuentas del usuario</returns>
    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva cuenta
    /// </summary>
    /// <param name="account">Cuenta a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta agregada</returns>
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza una cuenta existente
    /// </summary>
    /// <param name="account">Cuenta a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta actualizada</returns>
    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="account">Cuenta a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta eliminada</returns>
    public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe una cuenta con el número especificado
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    public async Task<bool> ExistsByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AnyAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }


} 