using System.Net;
using System.Text.Json;
using IMT_Reservas.Server.Shared.Common;

namespace IMT_Reservas.Server.Presentations.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        (int statusCode, string message) = exception switch
        {
            ErrorRegistroNoEncontrado => (404, "No se encontró el registro especificado"),
            ErrorCarnetUsuarioNoEncontrado => (404, "El carnet de usuario no fue encontrado"),
            ErrorGrupoEquipoNoEncontrado => (404, "El grupo de equipos no fue encontrado"),
            ErrorCategoriaNoEncontrada => (404, "La categoría no fue encontrada"),
            ErrorCarreraNoEncontrada => (404, "La carrera no fue encontrada"),
            ErrorCodigoImtNoEncontrado => (404, "El código IMT no fue encontrado"),
            ErrorEmpresaMantenimientoNoEncontrada => (404, "La empresa de mantenimiento no fue encontrada"),
            ErrorGaveteroNoEncontrado => (404, "El gavetero no fue encontrado"),
            ErrorMuebleNoEncontrado => (404, "El mueble no fue encontrado"),

            ErrorRegistroYaExiste => (409, "Ya existe un registro con estos datos"),
            ErrorRegistroEnUso => (409, "No se puede eliminar el registro porque está siendo utilizado"),
            ErrorNoEquiposDisponibles => (409, "No hay equipos disponibles para las fechas solicitadas"),

            ErrorUsuarioNoAutorizado => (401, "El usuario no está autorizado para realizar esta acción"),

            ErrorDataBase ex => (500, $"Error de base de datos: {ex.Message}"),
            ErrorRepository ex => (500, $"Error del repositorio: {ex.Message}"),

            _ => (500, exception.Message)
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = exception.GetType().Name,
            mensaje = message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
