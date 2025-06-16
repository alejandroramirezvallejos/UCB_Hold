using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly ComponenteService servicio;

    public ComponenteController(ComponenteService servicio)
    {
        this.servicio = servicio;
    }
    [HttpPost]
    public ActionResult Crear([FromBody] CrearComponenteComando input)
    {
        try
        {
            servicio.CrearComponente(input);
            return Created();
        }
        catch (Exception ex)
        {
            var fullError = ex.InnerException?.Message ?? ex.Message;
            return BadRequest($"Error al crear componente: {fullError}");
        }
    }

    [HttpGet]
    public ActionResult<List<ComponenteDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodosComponentes();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener componentes: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarComponenteComando input)
    {
        try
        {
            servicio.ActualizarComponente(input);
            return Ok("Componente actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar componente: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {

            var comando = new EliminarComponenteComando(id);
            servicio.EliminarComponente(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar componente: {ex.Message}");
        }
    }
}