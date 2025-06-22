using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly IGaveteroService servicio;
    public GaveteroController(IGaveteroService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearGaveteroComando input)
    {
        try { 
            servicio.CrearGavetero(input); 
            return Created(); 
        }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosGaveteros()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGaveteroComando input)
    {
        try { 
            servicio.ActualizarGavetero(input); 
            return Ok(new { mensaje = "Gavetero actualizado exitosamente" }); 
        }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorValorNegativo ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { 
            servicio.EliminarGavetero(new EliminarGaveteroComando(id)); 
            return NoContent(); 
        }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}