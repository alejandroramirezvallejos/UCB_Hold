using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly ICrearGrupoEquipoComando                       _crear;
    private readonly IObtenerGrupoEquipoConsulta                    _obtener;
    private readonly IActualizarGrupoEquipoComando                  _actualizar;
    private readonly IEliminarGrupoEquipoComando                    _eliminar;
    private readonly IObtenerGrupoEquipoPorIdConsulta               _obtenerPorId;
    private readonly IObtenerGrupoEquipoPorNombreYCategoriaConsulta _obtenerPorNombreYCategoria;

    public GrupoEquipoController(ICrearGrupoEquipoComando crear, IObtenerGrupoEquipoConsulta obtener,
                                IActualizarGrupoEquipoComando actualizar, IEliminarGrupoEquipoComando eliminar,
                                IObtenerGrupoEquipoPorIdConsulta obtenerPorId,
                                IObtenerGrupoEquipoPorNombreYCategoriaConsulta obtenerPorNombreYCategoria)
    {
        _crear                      = crear;
        _obtener                    = obtener;
        _actualizar                 = actualizar;
        _eliminar                   = eliminar;
        _obtenerPorId               = obtenerPorId;
        _obtenerPorNombreYCategoria = obtenerPorNombreYCategoria;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] GrupoEquipoRequestDto dto)
    {
        try
        {
            if (dto == null)
                return BadRequest("Los datos del grupo de equipo son requeridos");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del grupo de equipo es requerido");

            if (string.IsNullOrWhiteSpace(dto.Modelo))
                return BadRequest("El modelo del grupo de equipo es requerido");

            if (string.IsNullOrWhiteSpace(dto.Marca))
                return BadRequest("La marca del grupo de equipo es requerida");

            if (string.IsNullOrWhiteSpace(dto.NombreCategoria))
                return BadRequest("El nombre de la categoría es requerido");

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                return BadRequest("La descripción del grupo de equipo es requerida");

            if (string.IsNullOrWhiteSpace(dto.UrlImagen))
                return BadRequest("La URL de la imagen es requerida");

            var comando = new CrearGrupoEquipoComando(
                dto.Nombre,
                dto.Modelo,
                dto.Marca,
                dto.Descripcion,
                dto.NombreCategoria,
                dto.UrlDataSheet,
                dto.UrlImagen
            );

            _crear.Handle(comando);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al crear grupo de equipo: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<GrupoEquipoDto>> ObtenerTodos()
    {
        try
        {
            var resultado = _obtener.Handle();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener grupos de equipo: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<GrupoEquipoDto> ObtenerPorId([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var consulta = new ObtenerGrupoEquipoPorIdConsulta(id);
            var resultado = _obtenerPorId.Handle(consulta);

            if (resultado == null)
                return NotFound($"No se encontró el grupo de equipo con ID {id}");

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener grupo de equipo por ID: {ex.Message}");
        }
    }

    [HttpGet("buscar")]
    public ActionResult<List<GrupoEquipoDto>> ObtenerPorNombreYCategoria([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorNombreYCategoriaConsulta(nombre, categoria);
            var resultado = _obtenerPorNombreYCategoria.Handle(consulta);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al buscar grupos de equipo: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar([Range(1, int.MaxValue)] int id, [FromBody] GrupoEquipoRequestDto dto)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            if (dto == null)
                return BadRequest("Los datos del grupo de equipo son requeridos");

            var comando = new ActualizarGrupoEquipoComando(
                id,
                dto.Nombre,
                dto.Modelo,
                dto.Marca,
                dto.Descripcion,
                dto.NombreCategoria,
                dto.UrlDataSheet,
                dto.UrlImagen
            );

            _actualizar.Handle(comando);
            return Ok("Grupo de equipo actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar grupo de equipo: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar([Range(1, int.MaxValue)] int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarGrupoEquipoComando(id);
            _eliminar.Handle(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar grupo de equipo: {ex.Message}");
        }
    }
}

