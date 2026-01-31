using System.Net;
using System.Text.Json;

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
        
        var response = new ErrorResponse();
        
        switch (exception)
        {
            case ErrorRegistroNoEncontrado:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Error = "No encontrado";
                response.Mensaje = exception.Message;
                break;
            case ErrorRegistroYaExiste:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Error = "Conflicto";
                response.Mensaje = exception.Message;
                break;
            case ErrorRegistroEnUso:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Error = "Recurso en uso";
                response.Mensaje = exception.Message;
                break;
            case ErrorDataBase:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Error = "Error de datos";
                
                response.Mensaje = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Error al procesar la solicitud";
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Error = "Error interno";
                
                response.Mensaje = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Ha ocurrido un error inesperado";
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = null };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
