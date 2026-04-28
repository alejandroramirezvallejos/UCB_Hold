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
        
        int statusCode = (int)HttpStatusCode.BadRequest; // Default to 400

        if (exception is ErrorRegistroNoEncontrado || exception is ErrorCarnetUsuarioNoEncontrado || exception is ErrorCarreraNoEncontrada || exception is ErrorCategoriaNoEncontrada || exception is ErrorCodigoImtNoEncontrado || exception is ErrorEmpresaMantenimientoNoEncontrada || exception is ErrorGaveteroNoEncontrado || exception is ErrorGrupoEquipoNoEncontrado || exception is ErrorMuebleNoEncontrado)
        {
            statusCode = (int)HttpStatusCode.NotFound;
        }
        else if (exception is ErrorRegistroYaExiste || exception is ErrorRegistroEnUso || exception is ErrorNoEquiposDisponibles)
        {
            statusCode = (int)HttpStatusCode.Conflict;
        }
        else if (exception is ErrorUsuarioNoAutorizado)
        {
            statusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception is ErrorDataBase || exception.Message.Contains("Error General Servidor") || exception.InnerException?.Message.Contains("Error General Servidor") == true)
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
        }
        else if (!(exception is DomainException) && !(exception is ArgumentException) && !(exception is ArgumentNullException))
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.Response.StatusCode = statusCode;

        var response = new 
        { 
            error = exception.GetType().Name, 
            mensaje = exception.Message 
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
