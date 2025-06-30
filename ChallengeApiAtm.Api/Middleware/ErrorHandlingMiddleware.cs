using ChallengeApiAtm.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace ChallengeApiAtm.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del middleware de manejo de errores
    /// </summary>
    /// <param name="next">Siguiente middleware en el pipeline</param>
    /// <param name="logger">Logger para registrar errores</param>
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    
    /// <summary>
    /// Procesa la solicitud y maneja errores de manera centralizada
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Maneja las excepciones y devuelve respuestas apropiadas
    /// </summary>
    /// <param name="context">Contexto HTTP</param>
    /// <param name="exception">Excepción a manejar</param>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response = exception switch
        {
            InvalidPinException ex => new
            {
                error = "PIN_INCORRECTO",
                message = ex.Message,
                remainingAttempts = ex.RemainingAttempts,
                statusCode = HttpStatusCode.Unauthorized
            },
            CardBlockedException ex => new
            {
                error = "TARJETA_BLOQUEADA",
                message = ex.Message,
                cardNumber = ex.CardNumber,
                statusCode = HttpStatusCode.Forbidden
            },
            InsufficientFundsException ex => new
            {
                error = "FONDOS_INSUFICIENTES",
                message = ex.Message,
                requestedAmount = ex.RequestedAmount,
                availableBalance = ex.AvailableBalance,
                statusCode = HttpStatusCode.BadRequest
            },
            ArgumentException ex => new
            {
                error = "PARAMETRO_INVALIDO",
                message = ex.Message,
                parameter = ex.ParamName,
                statusCode = HttpStatusCode.BadRequest
            },
            InvalidOperationException ex => new
            {
                error = "OPERACION_INVALIDA",
                message = ex.Message,
                statusCode = HttpStatusCode.BadRequest
            },
            UnauthorizedAccessException => new
            {
                error = "NO_AUTORIZADO",
                message = "No tiene autorización para realizar esta operación",
                statusCode = HttpStatusCode.Unauthorized
            },
            _ => new
            {
                error = "ERROR_INTERNO",
                message = "Se produjo un error interno del servidor",
                statusCode = HttpStatusCode.InternalServerError
            }
        };

        var statusCodeProperty = response.GetType().GetProperty("statusCode");
        var statusCode = (HttpStatusCode)(statusCodeProperty?.GetValue(response) ?? HttpStatusCode.InternalServerError);
        context.Response.StatusCode = (int)statusCode;

        var errorProperty = response.GetType().GetProperty("error");
        var messageProperty = response.GetType().GetProperty("message");
        
        var error = errorProperty?.GetValue(response)?.ToString() ?? "ERROR_DESCONOCIDO";
        var message = messageProperty?.GetValue(response)?.ToString() ?? "Error desconocido";

        var jsonResponse = JsonSerializer.Serialize(new
        {
            success = false,
            timestamp = DateTime.UtcNow,
            error,
            message,
            details = response.GetType().GetProperties()
                .Where(p => p.Name != "error" && p.Name != "message" && p.Name != "statusCode")
                .ToDictionary(p => p.Name, p => p.GetValue(response))
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
} 