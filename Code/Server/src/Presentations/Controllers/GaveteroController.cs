using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly IGaveteroService servicio;

    public GaveteroController(IGaveteroService servicio)
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
        catch(ErrorNombreMuebleRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Gavetero duplicado", mensaje = ex.Message });
        }        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el gavetero" });
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
        }        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }        catch (ErrorNombreMuebleRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Gavetero no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Gavetero duplicado", mensaje = ex.Message });
        }        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }catch(ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el gavetero" });
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
        }        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Gavetero no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroEnUso ex)
        {
            return Conflict(new { error = "Registro en uso", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el gavetero" });
        }
    }
}