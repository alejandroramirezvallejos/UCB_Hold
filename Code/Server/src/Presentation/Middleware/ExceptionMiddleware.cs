using IMT_Reservas.Server.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace IMT_Reservas.Server.Presentation.Middleware;

public class ExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = null };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Recurso no encontrado: {Message}", ex.Message);
            await HandleException(context, 404, [ex.Message]);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Conflicto de base de datos: {Message}", ex.Message);
            await HandleException(context, 409, ["Conflicto al guardar: registro duplicado o restricción violada"]);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Conflicto: {Message}", ex.Message);
            await HandleException(context, 409, [ex.Message]);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validación: {Message}", ex.Message);
            await HandleException(context, 400, [ex.Message]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await HandleException(context, 500, ["Error interno del servidor. Por favor intenta de nuevo más tarde."]);
        }
    }

    private Task HandleException(HttpContext context, int statusCode, List<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new Response<object>
        {
            Status = statusCode,
            Value = null,
            Errors = errors,
            ValidationErrors = []
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
