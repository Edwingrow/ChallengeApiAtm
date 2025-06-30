using ChallengeApiAtm.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ChallengeApiAtm.Infrastructure.Services;

/// <summary>
/// Servicio para obtener información del usuario autenticado desde el contexto HTTP
/// </summary>
public sealed class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtTokenService _jwtTokenService;

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        IJwtTokenService jwtTokenService)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    public string? GetAuthenticatedCardNumber()
    {
        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
            return null;

        return _jwtTokenService.GetCardNumberFromToken(token);
    }

    public Guid? GetAuthenticatedUserId()
    {
        var token = GetTokenFromRequest();
        if (string.IsNullOrEmpty(token))
            return null;

        return _jwtTokenService.GetUserIdFromToken(token);
    }

    public bool ValidateCardOwnership(string requestedCardNumber)
    {
        if (string.IsNullOrEmpty(requestedCardNumber))
            return false;

        var authenticatedCardNumber = GetAuthenticatedCardNumber();
        if (string.IsNullOrEmpty(authenticatedCardNumber))
            return false;

        return authenticatedCardNumber == requestedCardNumber;
    }

    private string? GetTokenFromRequest()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return null;

        return authHeader["Bearer ".Length..];
    }
} 