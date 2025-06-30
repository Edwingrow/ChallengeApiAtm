using ChallengeApiAtm.Domain.Entities;

namespace ChallengeApiAtm.Domain.Interfaces;

/// <summary>
/// Contrato para el repositorio de cuentas
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Obtiene una cuenta por su ID
    /// </summary>
    /// <param name="id">ID de la cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta encontrada o null</returns>
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una cuenta por su número
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta encontrada o null</returns>
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una cuenta por su número incluyendo datos del usuario
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Cuenta con datos del usuario o null</returns>
    Task<Account?> GetByAccountNumberWithUserAsync(string accountNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las cuentas de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de cuentas del usuario</returns>
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva cuenta
    /// </summary>
    /// <param name="account">Cuenta a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Account account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una cuenta existente
    /// </summary>
    /// <param name="account">Cuenta a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una cuenta
    /// </summary>
    /// <param name="account">Cuenta a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Account account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una cuenta con el número especificado
    /// </summary>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe, False en caso contrario</returns>
    Task<bool> ExistsByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
} 