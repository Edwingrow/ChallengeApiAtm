using ChallengeApiAtm.Application.DTOs.Requests;
using ChallengeApiAtm.Application.DTOs.Responses;
using ChallengeApiAtm.Application.Helpers;
using ChallengeApiAtm.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChallengeApiAtm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class AccountController : ControllerBase
{
    private readonly IAtmService _atmService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAtmService atmService,
        ILogger<AccountController> logger)
    {
        _atmService = atmService ?? throw new ArgumentNullException(nameof(atmService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Consulta el saldo de una cuenta
    /// </summary>
    /// <param name="request">Datos de la consulta de saldo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del saldo y cuenta</returns>
    [HttpPost("balance")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<BalanceResponse>> GetBalance(
        [FromBody] BalanceRequest request,
        CancellationToken cancellationToken = default)
    {

        _logger.LogInformation($"Consulta de saldo para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        var response = await _atmService.GetBalanceAsync(request, cancellationToken);

        _logger.LogInformation($"Consulta de saldo exitosa para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        return Ok(new
        {
            success = true,
            data = response,
            message = "Consulta de saldo realizada exitosamente"
        });
    }

    /// <summary>
    /// Realiza un retiro de dinero
    /// </summary>
    /// <param name="request">Datos del retiro</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del retiro</returns>
    [HttpPost("withdraw")]
    [ProducesResponseType(typeof(WithdrawResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WithdrawResponse>> Withdraw(
        [FromBody] WithdrawRequest request,
        CancellationToken cancellationToken = default)
    {

        _logger.LogInformation($"Intento de retiro de ${request.Amount} para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        var response = await _atmService.WithdrawAsync(request, cancellationToken);

        _logger.LogInformation($"Retiro exitoso de ${request.Amount} para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}. Transacción: {response.TransactionId}",
            request.Amount, CardHelper.MaskCardNumber(request.CardNumber), response.TransactionId);

        return Ok(new
        {
            success = true,
            data = response,
            message = $"Retiro de ${request.Amount:F2} realizado exitosamente"
        });
    }
    /// <summary>
    /// Obtiene el historial de operaciones de una cuenta
    /// </summary>
    /// <param name="request">Datos de la consulta</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Historial de operaciones</returns>
    [HttpPost("operations")]
    [ProducesResponseType(typeof(OperationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<OperationsResponse>> GetOperations(
        [FromBody] OperationsRequest request,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Consulta de operaciones para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}, Página: {pageNumber}, Tamaño: {pageSize}");

        var response = await _atmService.GetOperationsAsync(request, pageNumber, pageSize, cancellationToken);

        _logger.LogInformation($"Consulta de operaciones exitosa para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        return Ok(new
        {
            success = true,
            data = response
        });
    }

  
   
}