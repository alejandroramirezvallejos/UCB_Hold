using Microsoft.AspNetCore.Mvc;
using API.ViewModels;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly GrupoEquipoService servicio;

    public GrupoEquipoController(GrupoEquipoService servicio)
    {
        this.servicio = servicio;
    }

    [HttpPost]
    public ActionResult Crear([FromBody] CrearGrupoEquipoComando input)
    {
        try
        {
            servicio.CrearGrupoEquipo(input);
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
            var resultado = servicio.ObtenerTodosGruposEquipos();
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener grupos de equipo: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public ActionResult<GrupoEquipoDto> ObtenerPorId(int id)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorIdConsulta(id);
            var resultado = servicio.ObtenerGrupoEquipoPorId(consulta);

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
            var resultado = servicio.ObtenerGrupoEquipoPorNombreYCategoria(consulta);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al buscar grupos de equipo: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public ActionResult Actualizar(int id, [FromBody] GrupoEquipoRequestDto dto)
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

            servicio.ActualizarGrupoEquipo(comando);
            return Ok("Grupo de equipo actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al actualizar grupo de equipo: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Eliminar(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0");

            var comando = new EliminarGrupoEquipoComando(id);
            servicio.EliminarGrupoEquipo(comando);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al eliminar grupo de equipo: {ex.Message}");
        }
    }
}

