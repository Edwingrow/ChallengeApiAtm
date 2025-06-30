using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;

namespace ChallengeApiAtm.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de operaciones del ATM
/// </summary>
public interface IAtmService
{
    /// <summary>
    /// Consulta el saldo de una cuenta
    /// </summary>
    /// <param name="request">Datos de la consulta de saldo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del saldo y cuenta</returns>
    Task<BalanceResponse> GetBalanceAsync(BalanceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Realiza un retiro de dinero
    /// </summary>
    /// <param name="request">Datos del retiro</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información de la transacción realizada</returns>
    Task<WithdrawResponse> WithdrawAsync(WithdrawRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial de operaciones paginado
    /// </summary>
    /// <param name="request">Parámetros de consulta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de transacciones</returns>
    Task<OperationsResponse> GetOperationsAsync(OperationsRequest request, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida que una tarjeta existe y está activa
    /// </summary>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si la tarjeta es válida</returns>
    Task<bool> IsCardValidAsync(string cardNumber, CancellationToken cancellationToken = default);
} 