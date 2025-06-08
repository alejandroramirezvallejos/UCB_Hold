using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ICrearCategoriaComando      _crear;
    private readonly IObtenerCategoriaConsulta   _obtener;
    private readonly IActualizarCategoriaComando _actualizar;
    private readonly IEliminarCategoriaComando   _eliminar;

    public CategoriaController(ICrearCategoriaComando crear, IObtenerCategoriaConsulta obtener,
                               IActualizarCategoriaComando actualizar,
                               IEliminarCategoriaComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CategoriaRequestDto dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre de la categoría es requerido");

            var comando = new CrearCategoriaComando(dto.Nombre);
            _crear.Handle(comando);
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
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener categorías: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] CategoriaRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre de la categoría es requerido");

            var comando = new ActualizarCategoriaComando(id, dto.Nombre);
            _actualizar.Handle(comando);
            
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
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarCategoriaComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar categoría: {ex.Message}");
        }
    }
}
