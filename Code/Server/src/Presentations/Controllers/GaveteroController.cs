using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly GaveteroService servicio;

    public GaveteroController(GaveteroService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearGaveteroComando input)
    {
        try
        {
            servicio.CrearGavetero(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear gavetero: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<GaveteroDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosGaveteros();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener gaveteros: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarGaveteroComando input)
    {
        try
        {
            servicio.ActualizarGavetero(input);
            return Ok("Gavetero actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar gavetero: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarGaveteroComando(id);
            servicio.EliminarGavetero(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar gavetero: {ex.Message}");
        }
    }
}