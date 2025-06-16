using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly CarreraService servicio;
    public CarreraController(CarreraService servicio)
    {
        this.servicio = servicio;
    }
    [HttpPost]
    public ActionResult Crear([FromBody] CrearCarreraComando input)
    {
        try
        {
            servicio.CrearCarrera(input);

            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear carrera: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<CarreraDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodasCarreras();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener carreras: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarCarreraComando input)
    {
        try
        {
            servicio.ActualizarCarrera(input);
            return Ok("Carrera actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar carrera: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarCarreraComando(id);
            servicio.EliminarCarrera(comando);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar carrera: {ex.Message}");
        }
    }
}