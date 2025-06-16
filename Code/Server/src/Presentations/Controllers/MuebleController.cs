using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
    private readonly MuebleService servicio;

    public MuebleController(MuebleService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearMuebleComando input)
    {
        try
        {
            servicio.CrearMueble(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear mueble: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<MuebleDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosMuebles();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener muebles: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarMuebleComando input)
    {
        try
        {
            servicio.ActualizarMueble(input);
            return Ok("Mueble actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar mueble: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarMuebleComando(id);
            servicio.EliminarMueble(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar mueble: {ex.Message}");
        }
    }
}