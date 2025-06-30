using ChallengeApiAtm.Domain.Entities;
using ChallengeApiAtm.Domain.Interfaces;
using ChallengeApiAtm.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChallengeApiAtm.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de usuarios usando Entity Framework
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly AtmDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de usuarios
    /// </summary>
    /// <param name="context">Contexto de base de datos</param>
    public UserRepository(AtmDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario encontrado o null</returns>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Obtiene un usuario por su número de documento
    /// </summary>
    /// <param name="documentNumber">Número de documento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario encontrado o null</returns>
    public async Task<User?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.DocumentNumber == documentNumber, cancellationToken);
    }

    /// <summary>
    /// Agrega un nuevo usuario
    /// </summary>
    /// <param name="user">Usuario a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario agregado</returns>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    /// <param name="user">Usuario a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario actualizado</returns>
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un usuario
    /// </summary>
    /// <param name="user">Usuario a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario eliminado</returns>
    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe un usuario con el número de documento especificado
    /// </summary>
    /// <param name="documentNumber">Número de documento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.DocumentNumber == documentNumber, cancellationToken);
    }
} 