using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly ICarreraService _servicio;
    public CarreraController(ICarreraService servicio) => _servicio = servicio;

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(_servicio.ObtenerTodasCarreras()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CrearCarreraComando input)
    {
        try { _servicio.CrearCarrera(input); return Created($"api/carrera/{input.Nombre}", input); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCarreraComando input)
    {
        try { _servicio.ActualizarCarrera(input); return Ok(new { mensaje = "Carrera actualizada exitosamente" }); }
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
        try { _servicio.EliminarCarrera(new EliminarCarreraComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }
}