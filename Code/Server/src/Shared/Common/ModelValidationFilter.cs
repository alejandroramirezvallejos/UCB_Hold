using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Common
{
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errores = new List<string>();
                
                foreach (var modelError in context.ModelState.Values.SelectMany(v => v.Errors))
                {
                    errores.Add(modelError.ErrorMessage);
                }
                
                // Usar tu sistema de excepciones de dominio
                var mensajeError = string.Join(". ", errores);
                
                // Decidir qué tipo de error según el contenido
                if (mensajeError.Contains("required") || mensajeError.Contains("requerido"))
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        tipo = "ErrorNombreRequerido",
                        mensaje = mensajeError,
                        errores = errores
                    });
                    return;
                }
                
                if (mensajeError.Contains("not valid") || mensajeError.Contains("no válido"))
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        tipo = "ErrorValorInvalido", 
                        mensaje = mensajeError,
                        errores = errores
                    });
                    return;
                }
                
                // Error genérico de validación
                context.Result = new BadRequestObjectResult(new
                {
                    tipo = "ErrorValidacion",
                    mensaje = "Los datos enviados no son válidos",
                    errores = errores
                });
            }
        }
    }
}
