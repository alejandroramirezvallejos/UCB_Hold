using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly MantenimientoService servicio;

    public MantenimientoController(MantenimientoService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearMantenimientoComando input)
    {
        try
        {
            servicio.CrearMantenimiento(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear mantenimiento: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<MantenimientoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosMantenimientos();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener mantenimientos: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarMantenimientoComando(id);
            servicio.EliminarMantenimiento(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar mantenimiento: {ex.Message}");
        }
    }
}