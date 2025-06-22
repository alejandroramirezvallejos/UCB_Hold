using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Interfaces;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentarioController : ControllerBase
{
    private readonly IComentarioService servicio;
    public ComentarioController(IComentarioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearComentarioComando input)
    {
        try { servicio.CrearComentario(input); return Created("", new { mensaje = "Comentario creado exitosamente" }); }
        catch (Exception ex) {
            if (ex.Message.Contains("Error General Servidor") || ex.InnerException?.Message.Contains("Error General Servidor") == true)
                return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpGet("grupo/{idGrupoEquipo}")]
    public IActionResult ObtenerComentariosPorGrupoEquipo(int idGrupoEquipo)
    {
        try {
            var consulta = new ObtenerComentariosPorGrupoEquipoConsulta(idGrupoEquipo);
            var resultado = servicio.ObtenerComentariosPorGrupoEquipo(consulta);
            if (resultado == null || resultado.Count == 0)
                return NotFound(new { error = "NoEncontrado", mensaje = "No se encontraron comentarios para el grupo." });
            return Ok(resultado);
        }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        try { servicio.EliminarComentario(new EliminarComentarioComando(id)); return NoContent(); }
        catch (ErrorUsuarioNoAutorizado) { return Forbid(); }
        catch (ErrorRegistroNoEncontrado) { return NotFound(new { error = "NoEncontrado", mensaje = "Comentario no encontrado" }); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPost("{id}/like")]
    public IActionResult AgregarMeGusta(string id)
    {
        try { servicio.AgregarLikeComentario(new AgregarLikeComentarioComando(id)); return Ok(new { mensaje = "Like agregado exitosamente al comentario" }); }
        catch (ErrorRegistroNoEncontrado) { return NotFound(new { error = "NoEncontrado", mensaje = "Comentario no encontrado" }); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}