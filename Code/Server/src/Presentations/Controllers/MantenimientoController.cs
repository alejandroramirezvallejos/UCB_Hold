using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly IMantenimientoService servicio;
    public MantenimientoController(IMantenimientoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearMantenimientoComando input)
    {
        try { 
            servicio.CrearMantenimiento(input); 
            return Created(); 
        }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorFechaInvalida ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorReferenciaInvalida ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosMantenimientos()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { 
            servicio.EliminarMantenimiento(new EliminarMantenimientoComando(id)); 
            return NoContent(); 
        }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}