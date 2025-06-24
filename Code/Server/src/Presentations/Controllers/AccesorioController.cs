using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly IAccesorioService servicio;
    public AccesorioController(IAccesorioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearAccesorioComando input)
    {
        try { servicio.CrearAccesorio(input); return Created("", new { message = "Accesorio creado exitosamente" }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosAccesorios()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarAccesorioComando input)
    {
        try { servicio.ActualizarAccesorio(input); return Ok(new { mensaje = "Accesorio actualizado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarAccesorio(new EliminarAccesorioComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }
}