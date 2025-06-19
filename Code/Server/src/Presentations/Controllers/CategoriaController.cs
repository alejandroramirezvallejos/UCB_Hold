using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICategoriaService servicio;

    public CategoriaController(ICategoriaService servicio)
    {
        this.servicio = servicio;
    }
    [HttpPost]
    public IActionResult Crear([FromBody] CrearCategoriaComando input)
    {
        try
        {
            servicio.CrearCategoria(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }        catch (ErrorLongitudInvalida ex)
        {
            return BadRequest(new { error = "Longitud inválida", mensaje = ex.Message });
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Categoría duplicada", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear la categoría" });
        }
    }

    [HttpGet]
    public ActionResult<List<CategoriaDto>> ObtenerTodos()
    {
        try
        {
            var resultado = servicio.ObtenerTodasCategorias();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener categorías: {ex.Message}" });
        }
    }    
    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarCategoriaComando input)
    {
        try
        {
            servicio.ActualizarCategoria(input);
            return Ok(new { mensaje = "Categoría actualizada exitosamente" });
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
        }        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Categoría no encontrada", mensaje = ex.Message });
        }        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Categoría duplicada", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar la categoría" });
        }
    }      [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarCategoriaComando(id);
            servicio.EliminarCategoria(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Categoría no encontrada", mensaje = ex.Message });
        }        catch (ErrorRegistroEnUso ex)
        {
            return Conflict(new { error = "Registro en uso", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception) 
        { 
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar la categoría" });
        }
    }
}
