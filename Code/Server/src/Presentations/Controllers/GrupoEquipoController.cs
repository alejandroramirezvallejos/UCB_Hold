using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Shared.Common;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly IGrupoEquipoService servicio;
    public GrupoEquipoController(IGrupoEquipoService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearGrupoEquipoComando input)
    {
        try { servicio.CrearGrupoEquipo(input); return Created(); }
        catch (ErrorRegistroYaExiste ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorModeloRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorCampoRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        try { return Ok(servicio.ObtenerTodosGruposEquipos()); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorIdConsulta(id);
            var resultado = servicio.ObtenerGrupoEquipoPorId(consulta);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpGet("buscar")]
    public IActionResult ObtenerPorNombreYCategoria([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        try
        {
            var consulta = new ObtenerGrupoEquipoPorNombreYCategoriaConsulta(nombre, categoria);
            var resultado = servicio.ObtenerGrupoEquipoPorNombreYCategoria(consulta);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpPut]
    public IActionResult Actualizar([FromBody] ActualizarGrupoEquipoComando input)
    {
        try { servicio.ActualizarGrupoEquipo(input); return Ok(new { mensaje = "Grupo de equipo actualizado exitosamente" }); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorNombreRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorModeloRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorCampoRequerido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorIdInvalido ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        try { servicio.EliminarGrupoEquipo(new EliminarGrupoEquipoComando(id)); return NoContent(); }
        catch (ErrorRegistroNoEncontrado ex) { return NotFound(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (ErrorRegistroEnUso ex) { return Conflict(new { error = ex.GetType().Name, mensaje = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
    
    
    [HttpGet("{carnetUsuario}/favoritos")]
    public IActionResult ObtenerFavoritosPorCarnetUsuario(string carnetUsuario)
    {
        try {
            var consulta = new ObtenerFavoritosPorCarnetUsuarioConsulta(carnetUsuario);
            return Ok(servicio.ObtenerFavoritosPorUsuario(consulta));
        }
        catch (Exception ex) {
            return BadRequest(new { mensaje = "Ocurrió un error al obtener los favoritos." });
        }
    }

    [HttpPost("{id}/favorito")]
    public IActionResult MarcarComoFavorito(int id, [FromQuery] string carnetUsuario)
    {
        try {
            servicio.MarcarComoFavorito(new MarcarComoFavoritoComando(id, carnetUsuario));
            return Ok(new { mensaje = "Equipo marcado como favorito" });
        }
        catch (Exception ex) {
            return BadRequest(new { mensaje = "Ocurrió un error al marcar como favorito." });
        }
    }
}
