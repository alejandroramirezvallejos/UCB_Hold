using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly AccesorioService servicio;

    public AccesorioController(AccesorioService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearAccesorioComando input)
    {
        try
        {
            servicio.CrearAccesorio(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear accesorio: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<AccesorioDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosAccesorios();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener accesorios: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarAccesorioComando input)
    {
        try
        {
            servicio.ActualizarAccesorio(input);
            return Ok("Accesorio actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar accesorio: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarAccesorioComando(id);
            servicio.EliminarAccesorio(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar accesorio: {ex.Message}");
        }
    }
}