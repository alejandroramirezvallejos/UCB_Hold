using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly CarreraService servicio;
    public CarreraController(CarreraService servicio)
    {
        this.servicio = servicio;
    }
    [HttpPost]
    public IActionResult Crear([FromBody] CrearCarreraComando input)
    {
        try
        {
            servicio.CrearCarrera(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message});
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Carrera duplicada", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear la carrera" });
        }
    }

    [HttpGet]
    public ActionResult<List<CarreraDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodasCarreras();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener carreras: {ex.Message}" });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCarreraComando input)
    {
        try
        {
            servicio.ActualizarCarrera(input);
            return Ok(new { mensaje = "Carrera actualizada exitosamente" });
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
            return NotFound(new { error = "Carrera no encontrada", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Carrera duplicada", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar la carrera" });
        }
    }    
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarCarreraComando(id);
            servicio.EliminarCarrera(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Carrera no encontrada", mensaje = $"No se encontró una carrera con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Carrera en uso", mensaje = "No se puede eliminar la carrera porque hay usuarios asociados a ella" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar la carrera" });
        }
    }
}