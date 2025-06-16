using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{    
    private readonly EquipoService servicio;

    public EquipoController(EquipoService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearEquipoComando input)
    {
        try
        {
            servicio.CrearEquipo(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear equipo: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<EquipoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosEquipos();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener equipos: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarEquipoComando input)
    {
        try
        {
            servicio.ActualizarEquipo(input);
            return Ok("Equipo actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar equipo: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarEquipoComando(id);
            servicio.EliminarEquipo(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar equipo: {ex.Message}");
        }
    }
}