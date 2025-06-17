using Microsoft.AspNetCore.Mvc;
using Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly EmpresaMantenimientoService servicio;

    public EmpresaMantenimientoController(EmpresaMantenimientoService servicio)
    {
        this.servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearEmpresaMantenimientoComando input)
    {
        try
        {
            servicio.CrearEmpresaMantenimiento(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Empresa duplicada", mensaje = $"Ya existe una empresa de mantenimiento con el nombre '{ex.Message}'" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear la empresa de mantenimiento" });
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
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener empresas de mantenimiento: {ex.Message}" });
        }
    }    
    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarEmpresaMantenimientoComando input)
    {
        try
        {
            servicio.ActualizarEmpresaMantenimiento(input);
            return Ok(new { mensaje = "Empresa de mantenimiento actualizada exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Empresa no encontrada", mensaje = $"No se encontró una empresa de mantenimiento con ID {ex.Message}" });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Empresa duplicada", mensaje = $"Ya existe otra empresa de mantenimiento con el nombre '{ex.Message}'" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar la empresa de mantenimiento" });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarEmpresaMantenimientoComando(id);
            servicio.EliminarEmpresaMantenimiento(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Empresa no encontrada", mensaje = $"No se encontró una empresa de mantenimiento con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Empresa en uso", mensaje = "No se puede eliminar la empresa porque tiene mantenimientos asociados" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception) { return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar la empresa de mantenimiento" });
        }
    }
}