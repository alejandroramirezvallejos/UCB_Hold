using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly ComponenteService servicio;

    public ComponenteController(ComponenteService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearComponenteComando input)
    {
        try
        {
            servicio.CrearComponente(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorComponenteYaExiste ex)
        {
            return Conflict(new { error = "Componente duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el componente" });
        }
    }

    [HttpGet]
    public ActionResult<List<ComponenteDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosComponentes();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener componentes: {ex.Message}" });
        }
    }
    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarComponenteComando input)
    {
        try
        {
            servicio.ActualizarComponente(input);
            return Ok(new { mensaje = "Componente actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Componente no encontrado", mensaje = $"No se encontró un componente con ID {ex.Message}" });
        }
        catch (ErrorComponenteYaExiste ex)
        {
            return Conflict(new { error = "Componente duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el componente" });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarComponenteComando(id);
            servicio.EliminarComponente(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Componente no encontrado", mensaje = $"No se encontró un componente con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Componente en uso", mensaje = "No se puede eliminar el componente porque está siendo utilizado" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el componente" });
        }
    }
}