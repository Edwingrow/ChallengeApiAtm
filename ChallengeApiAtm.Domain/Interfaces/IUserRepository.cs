using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.Domain.Interfaces;

/// <summary>
/// Contrato para el repositorio de usuarios
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario encontrado o null</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un usuario por su número de documento
    /// </summary>
    /// <param name="documentNumber">Número de documento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario encontrado o null</returns>
    Task<User?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo usuario
    /// </summary>
    /// <param name="user">Usuario a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    /// <param name="user">Usuario a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un usuario
    /// </summary>
    /// <param name="user">Usuario a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe un usuario con el número de documento especificado
    /// </summary>
    /// <param name="documentNumber">Número de documento</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
} 