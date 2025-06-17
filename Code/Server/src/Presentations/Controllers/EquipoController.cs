using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{    
    private readonly EquipoService servicio;

    public EquipoController(EquipoService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearEquipoComando input)
    {
        try
        {
            servicio.CrearEquipo(input);
            return Created();
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
            return Conflict(new { error = "Equipo duplicado", mensaje = $"Ya existe un equipo con código IMT '{ex.Message}'" });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el equipo" });
        }
    }

    [HttpGet]
    public ActionResult<List<EquipoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosEquipos();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener equipos: {ex.Message}" });
        }
    }    [HttpPut("{id}")]
    public IActionResult Actualizar([FromBody] ActualizarEquipoComando input)
    {
        try
        {
            servicio.ActualizarEquipo(input);
            return Ok(new { mensaje = "Equipo actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor negativo", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Equipo no encontrado", mensaje = $"No se encontró un equipo con código IMT {ex.Message}" });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el equipo" });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarEquipoComando(id);
            servicio.EliminarEquipo(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Equipo no encontrado", mensaje = $"No se encontró un equipo con código IMT {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Equipo en uso", mensaje = "No se puede eliminar el equipo porque está siendo utilizado en préstamos" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el equipo" });
        }
    }
}