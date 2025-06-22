using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly IComponenteService servicio;
    public ComponenteController(IComponenteService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearComponenteComando input)
    {
        try { servicio.CrearComponente(input); return Created("", new { message = "Componente creado exitosamente" }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return new BadRequestObjectResult(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosComponentes()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarComponenteComando input)
    {
        try { servicio.ActualizarComponente(input); return Ok(new { mensaje = "Componente actualizado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return new BadRequestObjectResult(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarComponente(new EliminarComponenteComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return new BadRequestObjectResult(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }
}