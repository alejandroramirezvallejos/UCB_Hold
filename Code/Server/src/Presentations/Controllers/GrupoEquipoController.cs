using Microsoft.AspNetCore.Mvc;
using Shared.Common;

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
    public IActionResult Crear([FromBody] CrearGrupoEquipoComando input)    {
        try
        {
            servicio.CrearGrupoEquipo(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste)
        {
            return Conflict(new { error = "Grupo de equipo duplicado", mensaje = $"Ya existe un grupo de equipo con ese nombre y características" });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el grupo de equipo" });
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
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener grupos de equipo: {ex.Message}" });
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
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al obtener grupo de equipo por ID: {ex.Message}" });
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
            return BadRequest(new { error = "Error interno del servidor", mensaje = $"Error al buscar grupos de equipo: {ex.Message}" });        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGrupoEquipoComando comando)    {
        try
        {
            if (comando == null)
                return BadRequest(new { error = "Datos requeridos", mensaje = "Los datos del grupo de equipo son requeridos" });
            servicio.ActualizarGrupoEquipo(comando);
            return Ok(new { mensaje = "Grupo de equipo actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Grupo de equipo no encontrado", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Grupo de equipo duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al actualizar el grupo de equipo" });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var comando = new EliminarGrupoEquipoComando(id);
            servicio.EliminarGrupoEquipo(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Grupo de equipo no encontrado", mensaje = $"No se encontró un grupo de equipo con ID {id}" });
        }
        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Grupo de equipo en uso", mensaje = "No se puede eliminar el grupo de equipo porque tiene equipos asociados o préstamos activos" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = "Error de validación", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el grupo de equipo" });
        }
    }
}

