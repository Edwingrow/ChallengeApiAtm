using Microsoft.AspNetCore.Mvc;
using ChallengeApiAtm.Infrastructure;

namespace ChallengeApiAtm.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public sealed class HealthController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HealthController> _logger;
    public HealthController(IServiceProvider serviceProvider, ILogger<HealthController> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var isDatabaseAvailable = await _serviceProvider.IsDatabaseAvailableAsync();

            var healthStatus = new
            {
                status = isDatabaseAvailable ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                checks = new
                {
                    database = isDatabaseAvailable ? "Connected" : "Disconnected",
                    api = "Running"
                }
            };

            if (!isDatabaseAvailable)
            {
                _logger.LogWarning("Health check failed: Database not available");
                return StatusCode(503, healthStatus);
            }

            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check");
            
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                error = "Internal health check error",
                checks = new
                {
                    database = "Error",
                    api = "Error"
                }
            });
        }
    }
} 