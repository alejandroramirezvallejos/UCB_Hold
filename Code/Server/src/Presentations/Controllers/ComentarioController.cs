using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentarioController : ControllerBase
{
    private readonly IComentarioService _servicio;

    public ComentarioController(IComentarioService servicio)
    {
        this._servicio = servicio;
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CrearComentarioComando input)
    {
        try
        {
            _servicio.CrearComentario(input);
            return Created("", new { mensaje = "Comentario creado exitosamente" });
        }
        catch (ErrorCampoRequerido ex)
        {
            return BadRequest(new { error = "Campo requerido", mensaje = ex.Message });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroYaExiste ex)
        {
            return Conflict(new { error = "Comentario duplicado", mensaje = ex.Message });
        }
        catch (ErrorReferenciaInvalida ex)
        {
            return BadRequest(new { error = "Referencia inválida", mensaje = ex.Message });
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = "Argumento inválido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al crear el comentario" });
        }
    }

    [HttpGet("grupo/{idGrupoEquipo}")]
    public ActionResult<List<ComentarioDto>> ObtenerComentariosPorGrupoEquipo(int idGrupoEquipo)
    {
        try
        {
            var consulta = new ObtenerComentariosPorGrupoEquipoConsulta(idGrupoEquipo);
            var resultado = _servicio.ObtenerComentariosPorGrupoEquipo(consulta);
            if (resultado == null || resultado.Count == 0)
            {
                return NotFound(new { mensaje = $"No se encontraron comentarios para el grupo de equipos con ID {idGrupoEquipo}" });
            }
            return Ok(resultado);
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al obtener los comentarios" });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        try
        {
            var comando = new EliminarComentarioComando(id);
            _servicio.EliminarComentario(comando);
            return NoContent();
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Comentario no encontrado", mensaje = ex.Message });
        }
        catch (ErrorUsuarioNoAutorizado ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { error = "Argumento requerido", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al eliminar el comentario" });
        }
    }

    [HttpPost("{id}/like")]
    public IActionResult AgregarMeGusta(string id)
    {
        try
        {
            var comando = new AgregarLikeComentarioComando(id);
            _servicio.AgregarLikeComentario(comando);
            return Ok(new { mensaje = "Like agregado exitosamente al comentario" });
        }
        catch (ErrorIdInvalido ex)
        {
            return BadRequest(new { error = "ID inválido", mensaje = ex.Message });
        }
        catch (ErrorRegistroNoEncontrado ex)
        {
            return NotFound(new { error = "Comentario no encontrado", mensaje = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno del servidor", mensaje = "Ocurrió un error inesperado al agregar like al comentario" });
        }
    }
}