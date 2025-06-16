using Microsoft.AspNetCore.Mvc;
using System.Data;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly CategoriaService servicio;

    public CategoriaController(CategoriaService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearCategoriaComando input)
    {
        try
        {
            servicio.CrearCategoria(input);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear categoría: {ex.Message}");
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
            return BadRequest($"Error al obtener categorías: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([FromBody] ActualizarCategoriaComando input)
    {
        try
        {
            servicio.ActualizarCategoria(input);
            return Ok("Categoría actualizada exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar categoría: {ex.Message}");
        }
    }
    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarCategoriaComando(id);
            servicio.EliminarCategoria(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar categoría: {ex.Message}");
        }
    }
}
