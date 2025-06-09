using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MuebleController : ControllerBase
{
    private readonly ICrearMuebleComando      _crear;
    private readonly IObtenerMuebleConsulta   _obtener;
    private readonly IActualizarMuebleComando _actualizar;
    private readonly IEliminarMuebleComando   _eliminar;

    public MuebleController(ICrearMuebleComando crear, IObtenerMuebleConsulta obtener,
                           IActualizarMuebleComando actualizar, IEliminarMuebleComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] MuebleRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del mueble son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del mueble es requerido");
            
            if (dto.Longitud.HasValue && dto.Longitud.Value < 0)
                return BadRequest("La longitud debe ser un número positivo");

            if (dto.Profundidad.HasValue && dto.Profundidad.Value < 0)
                return BadRequest("La profundidad debe ser un número positivo");

            if (dto.Altura.HasValue && dto.Altura.Value < 0)
                return BadRequest("La altura debe ser un número positivo");
            
            if (dto.Costo.HasValue && dto.Costo.Value < 0)
                return BadRequest("El costo debe ser un número positivo");

            var comando = new CrearMuebleComando(
                dto.Nombre,
                dto.Tipo,
                dto.Costo,
                dto.Ubicacion,
                dto.Longitud,
                dto.Profundidad,
                dto.Altura
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear mueble: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<MuebleDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener muebles: {ex.Message}");
        }
    }    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] MuebleRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del mueble son requeridos");
            
            if (dto.Longitud.HasValue && dto.Longitud.Value < 0)
                return BadRequest("La longitud debe ser un número positivo");

            if (dto.Profundidad.HasValue && dto.Profundidad.Value < 0)
                return BadRequest("La profundidad debe ser un número positivo");

            if (dto.Altura.HasValue && dto.Altura.Value < 0)
                return BadRequest("La altura debe ser un número positivo");
            
            if (dto.Costo.HasValue && dto.Costo.Value < 0)
                return BadRequest("El costo debe ser un número positivo");

            var comando = new ActualizarMuebleComando(
                id,
                dto.Nombre,
                dto.Tipo,
                dto.Costo,
                dto.Ubicacion,
                dto.Longitud,
                dto.Profundidad,
                dto.Altura
            );

            _actualizar.Handle(comando);
            return Ok("Mueble actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar mueble: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarMuebleComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar mueble: {ex.Message}");
        }
    }
}