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
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Intento de login para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        var response = await _authService.LoginAsync(request, cancellationToken);

        _logger.LogInformation($"Login exitoso para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        return Ok(new
        {
            success = true,
            timestamp = DateTime.UtcNow,
            data = response,
            message = "Autenticación exitosa"
        });
    }
    [HttpPost("unblock")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UnblockCardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnblockCardResponse>> UnblockCard(
        [FromBody] UnblockCardRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Intento de desbloqueo para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        var response = await _authService.UnblockCardAsync(request, cancellationToken);

        _logger.LogInformation($"Desbloqueo exitoso para tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");

        return Ok(new
        {
            success = true,
            timestamp = DateTime.UtcNow,
            data = response,
        });
    }

}