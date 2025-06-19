using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly ICarreraService _servicio;

    public CarreraController(ICarreraService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public ActionResult<List<CarreraDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _servicio.ObtenerTodasCarreras();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener carreras: {ex.Message}" });
        }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CrearCarreraComando input)
    {
        try
        {
            _servicio.CrearCarrera(input);
            return Created($"api/carrera/{input.Nombre}", input);
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message});
        }        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Carrera duplicada", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear la carrera" });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCarreraComando input)
    {
        try
        {
            _servicio.ActualizarCarrera(input);
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
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Carrera duplicada", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar la carrera" });
        }
    }      [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarCarreraComando(id);
            _servicio.EliminarCarrera(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Carrera no encontrada", mensaje = ex.Message });
        }
        catch (ErrorRegistroEnUso ex)
        {
            return Conflict(new { error = "Registro en uso", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar la carrera" });
        }
    }
}