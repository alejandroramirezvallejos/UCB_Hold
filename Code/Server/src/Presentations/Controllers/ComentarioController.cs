using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class ComentarioController : ControllerBase
{
    private readonly ComentarioService _servicio;
    public ComentarioController(ComentarioService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<ComentarioDto?> Crear([FromBody] CrearComentarioComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet("grupo/{idGrupoEquipo}")]
    public IActionResult ObtenerComentariosPorGrupoEquipo(int idGrupoEquipo)
    {
        var consulta = new ObtenerComentariosPorGrupoEquipoConsulta(idGrupoEquipo);
        var resultado = _servicio.ObtenerComentariosPorGrupoEquipo(consulta);
        if (resultado == null || resultado.Count == 0)
            return NotFound(new { error = "NoEncontrado", mensaje = "No se encontraron comentarios para el grupo." });
        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public Result<ComentarioDto?> Eliminar(string id)
    {
        return _servicio.Eliminar(new EliminarComentarioComando(id));
    }

    [HttpPost("{id}/like")]
    public IActionResult AgregarMeGusta(string id, [FromBody] AgregarLikeComentarioComando input)
    {
        _servicio.AgregarLikeComentario(new AgregarLikeComentarioComando(id, input.CarnetUsuario));
        return Ok(new { mensaje = "Like agregado exitosamente al comentario" });
    }

    [HttpDelete("{id}/like")]
    public IActionResult QuitarMeGusta(string id, [FromBody] QuitarLikeComentarioComando input)
    {
        _servicio.QuitarLikeComentario(new QuitarLikeComentarioComando(id, input.CarnetUsuario));
        return Ok(new { mensaje = "Like quitado exitosamente del comentario" });
    }
}
