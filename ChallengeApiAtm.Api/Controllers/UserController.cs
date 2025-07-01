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
public sealed class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Intento de registro para usuario: {request.DocumentNumber}, Tarjeta: {CardHelper.MaskCardNumber(request.CardNumber)}");
        
        var response = await _userService.RegisterUserAsync(request, cancellationToken);

        _logger.LogInformation($"Registro exitoso para usuario: {request.DocumentNumber}, Tarjeta: {CardHelper.MaskCardNumber(response.CardNumber)}");

        return CreatedAtAction(nameof(Register), new
        {
            success = true,
            data = response
        });
    }

} 