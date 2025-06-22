using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService servicio;
    public CategoriaController(ICategoriaService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearCategoriaComando input)
    {
        try { servicio.CrearCategoria(input); return Created(); }
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
        try { return Ok(servicio.ObtenerTodasCategorias()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCategoriaComando input)
    {
        try { servicio.ActualizarCategoria(input); return Ok(new { mensaje = "Categoría actualizada exitosamente" }); }
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
        try { servicio.EliminarCategoria(new EliminarCategoriaComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }
}
