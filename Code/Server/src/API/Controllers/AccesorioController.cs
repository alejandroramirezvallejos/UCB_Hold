using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly ICrearAccesorioComando      _crear;
    private readonly IObtenerAccesorioConsulta   _obtener;
    private readonly IActualizarAccesorioComando _actualizar;
    private readonly IEliminarAccesorioComando   _eliminar;

    public AccesorioController(ICrearAccesorioComando crear, IObtenerAccesorioConsulta obtener,
                               IActualizarAccesorioComando actualizar,
                               IEliminarAccesorioComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] AccesorioRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del accesorio son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del accesorio es requerido");
            
            if( string.IsNullOrWhiteSpace(dto.Modelo))
                return BadRequest("El modelo del accesorio es requerido");
            if(dto.CodigoIMT == null)
                return BadRequest("El código IMT del accesorio es requerido");

            var comando = new CrearAccesorioComando(
                dto.Nombre,
                dto.Modelo,
                dto.Tipo,
                (int)dto.CodigoIMT,
                dto.Descripcion,
                dto.Precio,
                dto.UrlDataSheet
            );

            _crear.Handle(comando);
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
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener accesorios: {ex.Message}");
        }
    }    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] AccesorioRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del accesorio son requeridos");

            var comando = new ActualizarAccesorioComando(
                id,
                dto.Nombre,
                dto.Modelo,
                dto.Tipo,
                dto.CodigoIMT,
                dto.Descripcion,
                dto.Precio,
                dto.UrlDataSheet
            );

            _actualizar.Handle(comando);
            return Ok("Accesorio actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar accesorio: {ex.Message}");
        }
    }    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarAccesorioComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar accesorio: {ex.Message}");
        }
    }
}