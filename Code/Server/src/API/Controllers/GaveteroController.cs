using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly ICrearGaveteroComando      _crear;
    private readonly IObtenerGaveteroConsulta   _obtener;
    private readonly IActualizarGaveteroComando _actualizar;
    private readonly IEliminarGaveteroComando   _eliminar;

    public GaveteroController(ICrearGaveteroComando crear, IObtenerGaveteroConsulta obtener,
                             IActualizarGaveteroComando actualizar, IEliminarGaveteroComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] GaveteroRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del gavetero son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del gavetero es requerido");

            if (string.IsNullOrWhiteSpace(dto.NombreMueble))
                return BadRequest("El nombre del mueble es requerido");

            var comando = new CrearGaveteroComando(
                dto.Nombre,
                dto.Tipo,
                dto.NombreMueble,
                dto.Longitud,
                dto.Profundidad,
                dto.Altura
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear gavetero: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<GaveteroDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener gaveteros: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([Range(1, int.MaxValue)] int id, [FromBody] GaveteroRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del gavetero son requeridos");

            var comando = new ActualizarGaveteroComando(
                id,
                dto.Nombre,
                dto.Tipo,
                dto.NombreMueble,
                dto.Longitud,
                dto.Profundidad,
                dto.Altura
            );

            _actualizar.Handle(comando);
            return Ok("Gavetero actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar gavetero: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarGaveteroComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar gavetero: {ex.Message}");
        }
    }
}