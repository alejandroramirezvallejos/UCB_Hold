using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly ICrearComponenteComando _crear;
    private readonly IObtenerComponenteConsulta _obtener;
    private readonly IActualizarComponenteComando _actualizar;
    private readonly IEliminarComponenteComando _eliminar;

    public ComponenteController(ICrearComponenteComando crear, IObtenerComponenteConsulta obtener,
                               IActualizarComponenteComando actualizar, IEliminarComponenteComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] ComponenteRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del componente son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del componente es requerido");

            var comando = new CrearComponenteComando(
                dto.Nombre,
                dto.Modelo,
                dto.Tipo,
                dto.CodigoIMT,
                dto.Descripcion,
                dto.PrecioReferencia,
                dto.UrlDataSheet
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear componente: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<ComponenteDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener componentes: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([Range(1, int.MaxValue)] int id, [FromBody] ComponenteRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del componente son requeridos");

            var comando = new ActualizarComponenteComando(
                id,
                dto.Nombre,
                dto.Modelo,
                dto.Tipo,
                dto.CodigoIMT,
                dto.Descripcion,
                dto.PrecioReferencia,
                dto.UrlDataSheet
            );

            _actualizar.Handle(comando);
            return Ok("Componente actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar componente: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarComponenteComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar componente: {ex.Message}");
        }
    }
}