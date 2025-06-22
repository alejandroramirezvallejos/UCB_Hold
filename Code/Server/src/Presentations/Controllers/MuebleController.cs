using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
    private readonly IMuebleService servicio;
    public MuebleController(IMuebleService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearMuebleComando input)
    {
        try { servicio.CrearMueble(input); return Created(); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosMuebles()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarMuebleComando input)
    {
        try { servicio.ActualizarMueble(input); return Ok(new { mensaje = "Mueble actualizado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarMueble(new EliminarMuebleComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}