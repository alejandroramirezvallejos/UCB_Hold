using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly EmpresaMantenimientoService servicio;

    public EmpresaMantenimientoController(EmpresaMantenimientoService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearEmpresaMantenimientoComando input)
    {
        try
        {
            servicio.CrearEmpresaMantenimiento(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear empresa de mantenimiento: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<EmpresaMantenimientoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodasEmpresasMantenimiento();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener empresas de mantenimiento: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarEmpresaMantenimientoComando input)
    {
        try
        {
            servicio.ActualizarEmpresaMantenimiento(input);
            return Ok("Empresa de mantenimiento actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar empresa de mantenimiento: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarEmpresaMantenimientoComando(id);
            servicio.EliminarEmpresaMantenimiento(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar empresa de mantenimiento: {ex.Message}");
        }
    }
}