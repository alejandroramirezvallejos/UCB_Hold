using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly IGrupoEquipoService _servicio;

    public GrupoEquipoController(IGrupoEquipoService servicio)
    {
        _servicio = servicio;
    }    [HttpPost]
    public IActionResult Crear([FromBody] CrearGrupoEquipoComando input)
    {
        try
        {
            _servicio.CrearGrupoEquipo(input);
            return Created();
        }
        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }        catch (ErrorModeloRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorMarcaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorDescripcionRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCategoriaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorUrlImagenRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCampoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Grupo de equipo duplicado", mensaje = ex.Message });
        }        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
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
            var resultado = _servicio.ObtenerTodosGruposEquipos();
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
            var resultado = _servicio.ObtenerGrupoEquipoPorId(consulta);

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
            var resultado = _servicio.ObtenerGrupoEquipoPorNombreYCategoria(consulta);
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
            _servicio.ActualizarGrupoEquipo(comando);
            return Ok(new { mensaje = "Grupo de equipo actualizado exitosamente" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorNombreRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorModeloRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorMarcaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorDescripcionRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCategoriaRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorUrlImagenRequerida ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorCampoRequerido ex)
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
        }        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
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
            _servicio.EliminarGrupoEquipo(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }        catch (ErrorRegistroNoEncontrado)
        {
            return NotFound(new { error = "Grupo de equipo no encontrado", mensaje = $"No se encontró un grupo de equipo con ID {id}" });
        }        catch (ErrorRegistroEnUso)
        {
            return Conflict(new { error = "Grupo de equipo en uso", mensaje = "No se puede eliminar el grupo de equipo porque tiene equipos asociados o préstamos activos" });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el grupo de equipo" });
        }
    }
}
