using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly AccesorioService servicio;

    public AccesorioController(AccesorioService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearAccesorioComando input)
    {
        try
        {
            servicio.CrearAccesorio(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorModeloRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorCodigoImtRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Registro duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el accesorio" });
        }
    }

    [HttpGet]
    public ActionResult<List<AccesorioDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosAccesorios();
            return Ok(resultado);
        }        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener accesorios: {ex.Message}" });
        }
    }    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarAccesorioComando input)
    {
        try
        {
            servicio.ActualizarAccesorio(input);
            return Ok(new { mensaje = "Accesorio actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorModeloRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorCodigoImtRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorValorNegativo ex)
        {
            return BadRequest(new { error = "Valor inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Accesorio no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Registro duplicado", mensaje = ex.Message });
        }        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el accesorio" });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarAccesorioComando(id);
            servicio.EliminarAccesorio(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Accesorio no encontrado", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el accesorio" });
        }
    }
}