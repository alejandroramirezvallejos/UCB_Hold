using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
    private readonly MuebleService servicio;

    public MuebleController(MuebleService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearMuebleComando input)
    {
        try
        {
            servicio.CrearMueble(input);
            return Created();
        }        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor negativo", mensaje = ex.Message });
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Mueble duplicado", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el mueble" });
        }
    }

    [HttpGet]
    public ActionResult<List<MuebleDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosMuebles();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener muebles: {ex.Message}" });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarMuebleComando input)
    {
        try
        {
            servicio.ActualizarMueble(input);
            return Ok(new { mensaje = "Mueble actualizado exitosamente" });
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
            return NotFound(new { error = "Mueble no encontrado", mensaje = ex.Message });
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Mueble duplicado", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el mueble" });
        }
    }    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarMuebleComando(id);
            servicio.EliminarMueble(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Mueble no encontrado", mensaje = $"No se encontró un mueble con ID {id}" });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el mueble" });
        }
    }
}