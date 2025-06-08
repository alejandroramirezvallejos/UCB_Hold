using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

            if (dto.GrupoEquipoId <= 0)
                return BadRequest("El ID del grupo de equipo es requerido");

            if (string.IsNullOrWhiteSpace(dto.CodigoImt))
                return BadRequest("El código IMT es requerido");

            if (string.IsNullOrWhiteSpace(dto.EstadoEquipo))
                return BadRequest("El estado del equipo es requerido");            // Verificar que el grupo de equipo existe
            var grupoEquipo = _obtenerGrupoEquipo.Handle(new ObtenerGrupoEquipoPorIdConsulta(dto.GrupoEquipoId));
            if (grupoEquipo == null)
                return BadRequest($"No existe el grupo de equipo con ID {dto.GrupoEquipoId}");

            var comando = new CrearEquipoComando(
                grupoEquipo.Nombre!,
                grupoEquipo.Modelo!,
                grupoEquipo.Marca!,
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
    public ActionResult Actualizar([Range(1, int.MaxValue)] int id, [FromBody] EquipoRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del equipo son requeridos");
            
            string? nombreGrupoEquipo = null;
            if (dto.GrupoEquipoId > 0)
            {
                var grupoEquipo = _obtenerGrupoEquipo.Handle(new ObtenerGrupoEquipoPorIdConsulta(dto.GrupoEquipoId));
                if (grupoEquipo == null)
                    return BadRequest($"No existe el grupo de equipo con ID {dto.GrupoEquipoId}");
                nombreGrupoEquipo = grupoEquipo.Nombre;
            }
            var comando = new ActualizarEquipoComando(
                id,
                nombreGrupoEquipo,
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
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
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