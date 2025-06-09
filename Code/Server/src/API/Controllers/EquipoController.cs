using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{    private readonly ICrearEquipoComando             _crear;
    private readonly IObtenerEquipoConsulta           _obtener;
    private readonly IActualizarEquipoComando         _actualizar;
    private readonly IEliminarEquipoComando           _eliminar;
    private readonly IObtenerGrupoEquipoPorIdConsulta _obtenerGrupoEquipo;

    public EquipoController(ICrearEquipoComando crear, IObtenerEquipoConsulta obtener,
                           IActualizarEquipoComando actualizar, IEliminarEquipoComando eliminar,
                           IObtenerGrupoEquipoPorIdConsulta obtenerGrupoEquipo)
    {
        _crear              = crear;
        _obtener            = obtener;
        _actualizar         = actualizar;
        _eliminar           = eliminar;
        _obtenerGrupoEquipo = obtenerGrupoEquipo;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] EquipoRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del equipo son requeridos");

            if (dto.NombreGrupoEquipo == null )
                return BadRequest("El nombre del grupo de equipo es requerido");
            if (string.IsNullOrWhiteSpace(dto.Modelo))
                return BadRequest("El modelo es requerido");
            if (string.IsNullOrWhiteSpace(dto.Marca))
                return BadRequest("La marca es requerida");

            var comando = new CrearEquipoComando(
                dto.NombreGrupoEquipo,
                dto.Modelo,
                dto.Marca,
                dto.CodigoUcb,
                dto.Descripcion,
                dto.NumeroSerial,
                dto.Ubicacion,
                dto.Procedencia,
                dto.CostoReferencia,
                dto.TiempoMaximoPrestamo,
                dto.NombreGavetero
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear equipo: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<EquipoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener equipos: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] EquipoRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del equipo son requeridos");
            
            var comando = new ActualizarEquipoComando(
                id,
                dto.NombreGrupoEquipo,
                dto.CodigoUcb,
                dto.Descripcion,
                dto.NumeroSerial,
                dto.Ubicacion,
                dto.Procedencia,
                dto.CostoReferencia,
                dto.TiempoMaximoPrestamo,
                dto.NombreGavetero,
                dto.EstadoEquipo
            );

            _actualizar.Handle(comando);
            return Ok("Equipo actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar equipo: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarEquipoComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar equipo: {ex.Message}");
        }
    }
}