using Shared.Common;
using System.Net;
using System.Text.Json;

namespace Shared.Common
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var result = exception switch
            {
                // Excepciones de validación de entrada
                ErrorNombreRequerido => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Required Field Error"
                },
                
                ErrorLongitudInvalida => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Length Validation Error"
                },
                
                ErrorIdInvalido => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Invalid ID Error"
                },
                
                ErrorValorNegativo => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Negative Value Error"
                },
                
                // Excepciones de base de datos
                ErrorRegistroYaExiste => new
                {
                    status = (int)HttpStatusCode.Conflict,
                    message = exception.Message,
                    type = "Duplicate Record Error"
                },
                
                ErrorRegistroNoEncontrado => new
                {
                    status = (int)HttpStatusCode.NotFound,
                    message = exception.Message,
                    type = "Record Not Found Error"
                },
                
                ErrorRegistroEnUso => new
                {
                    status = (int)HttpStatusCode.UnprocessableEntity,
                    message = exception.Message,
                    type = "Record In Use Error"
                },
                
                ErrorReferenciaInvalida => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Invalid Reference Error"
                },
                
                // Excepciones específicas de negocio
                ErrorCarnetUsuarioNoEncontrado => new
                {
                    status = (int)HttpStatusCode.NotFound,
                    message = exception.Message,
                    type = "User Not Found Error"
                },
                
                ErrorNoEquiposDisponibles => new
                {
                    status = (int)HttpStatusCode.UnprocessableEntity,
                    message = exception.Message,
                    type = "No Equipment Available Error"
                },
                
                ErrorComponenteYaExiste => new
                {
                    status = (int)HttpStatusCode.Conflict,
                    message = exception.Message,
                    type = "Component Already Exists Error"
                },
                
                // Excepciones genéricas
                ArgumentNullException => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = "Datos requeridos faltantes",
                    type = "Null Argument Error"
                },
                
                ArgumentException => new
                {
                    status = (int)HttpStatusCode.BadRequest,
                    message = exception.Message,
                    type = "Argument Error"
                },
                
                // Error por defecto
                _ => new
                {
                    status = (int)HttpStatusCode.InternalServerError,
                    message = "Error interno del servidor",
                    type = "Internal Server Error"
                }
            };

            response.StatusCode = result.status;
            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}
