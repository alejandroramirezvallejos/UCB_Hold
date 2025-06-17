using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly GaveteroService servicio;

    public GaveteroController(GaveteroService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearGaveteroComando input)
    {
        try
        {
            servicio.CrearGavetero(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor negativo", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Gavetero duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el gavetero" });
        }
    }

    [HttpGet]
    public ActionResult<List<GaveteroDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosGaveteros();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener gaveteros: {ex.Message}" });
        }
    }
    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGaveteroComando input)
    {
        try
        {
            servicio.ActualizarGavetero(input);
            return Ok(new { mensaje = "Gavetero actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor negativo", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Gavetero no encontrado", mensaje = $"No se encontró un gavetero con ID {ex.Message}" });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el gavetero" });
        }
    }    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarGaveteroComando(id);
            servicio.EliminarGavetero(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Gavetero no encontrado", mensaje = $"No se encontró un gavetero con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Gavetero en uso", mensaje = "No se puede eliminar el gavetero porque tiene equipos almacenados" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el gavetero" });
        }
    }
}