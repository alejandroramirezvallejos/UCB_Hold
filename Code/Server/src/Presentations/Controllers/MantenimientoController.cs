using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly MantenimientoService servicio;

    public MantenimientoController(MantenimientoService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearMantenimientoComando input)
    {
        try
        {
            servicio.CrearMantenimiento(input);
            return Created();
        }        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor negativo", mensaje = ex.Message });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el mantenimiento" });
        }
    }

    [HttpGet]
    public ActionResult<List<MantenimientoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosMantenimientos();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener mantenimientos: {ex.Message}" });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarMantenimientoComando(id);
            servicio.EliminarMantenimiento(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Mantenimiento no encontrado", mensaje = $"No se encontró un mantenimiento con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Mantenimiento en uso", mensaje = "No se puede eliminar el mantenimiento porque ya fue procesado" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el mantenimiento" });
        }
    }
}