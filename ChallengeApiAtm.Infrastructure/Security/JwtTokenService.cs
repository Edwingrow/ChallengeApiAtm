using ChallengeApiAtm.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChallengeApiAtm.Infrastructure.Security;

/// <summary>
/// Implementación del servicio de tokens JWT
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    /// <summary>
    /// Inicializa una nueva instancia del servicio JWT
    /// </summary>
    /// <param name="configuration">Configuración de la aplicación</param>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        _secretKey = _configuration["Jwt:SecretKey"] ?? "AtmSecretKey2024!@#$%^&*()_+1234567890SuperSecureKey";
        _issuer = _configuration["Jwt:Issuer"] ?? "ChallengeApiAtm";
        _audience = _configuration["Jwt:Audience"] ?? "ChallengeApiAtm-Users";
        _expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    /// <summary>
    /// Genera un token JWT
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="cardNumber">Número de tarjeta</param>
    /// <param name="accountNumber">Número de cuenta</param>
    /// <returns>Token JWT</returns>
    public string GenerateToken(Guid userId, string cardNumber, string accountNumber)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim("card_number", cardNumber),
            new Claim("account_number", accountNumber),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Valida un token JWT
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ClaimsPrincipal si el token es válido, null en caso contrario</returns>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            if (validatedToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtiene la duración del token en segundos
    /// </summary>
    /// <returns>Duración del token en segundos</returns>
    public int GetTokenExpirationInSeconds()
    {
        return _expirationMinutes * 60;
    }

    /// <summary>
    /// Obtiene el número de tarjeta del token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>Número de tarjeta o null si el token no es válido</returns>
    public string? GetCardNumberFromToken(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst("card_number")?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtiene el ID del usuario del token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID del usuario o null si el token no es válido</returns>
    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var principal = ValidateToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtiene los parámetros de validación del token
    /// </summary>
    /// <returns>Parámetros de validación</returns>
    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, 
            RequireExpirationTime = true
        };
    }
} 