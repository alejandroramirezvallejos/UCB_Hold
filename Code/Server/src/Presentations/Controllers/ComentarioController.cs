using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComentarioController : ControllerBase
{
    private readonly ComentarioService servicio;
    public ComentarioController(ComentarioService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearComentarioComando input)
    {
        servicio.Crear(input); return Created("", new { mensaje = "Comentario creado exitosamente" });
    }

    [HttpGet("grupo/{idGrupoEquipo}")]
    public IActionResult ObtenerComentariosPorGrupoEquipo(int idGrupoEquipo)
    {
        var consulta = new ObtenerComentariosPorGrupoEquipoConsulta(idGrupoEquipo);
        var resultado = servicio.ObtenerComentariosPorGrupoEquipo(consulta);
        if (resultado == null || resultado.Count == 0)
            return NotFound(new { error = "NoEncontrado", mensaje = "No se encontraron comentarios para el grupo." });
        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        servicio.Eliminar(new EliminarComentarioComando(id)); return NoContent();
    }

    [HttpPost("{id}/like")]
    public IActionResult AgregarMeGusta(string id, [FromBody] AgregarLikeComentarioComando input)
    {
        servicio.AgregarLikeComentario(new AgregarLikeComentarioComando(id, input.CarnetUsuario)); return Ok(new { mensaje = "Like agregado exitosamente al comentario" });
    }

    [HttpDelete("{id}/like")]
    public IActionResult QuitarMeGusta(string id, [FromBody] QuitarLikeComentarioComando input)
    {
        servicio.QuitarLikeComentario(new QuitarLikeComentarioComando(id, input.CarnetUsuario));
        return Ok(new { mensaje = "Like quitado exitosamente del comentario" });
    }
}