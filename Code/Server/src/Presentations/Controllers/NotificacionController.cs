using IMT_Reservas.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
    private readonly INotificacionService servicio;
    public NotificacionController(INotificacionService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearNotificacionComando comando)
    {
        try { servicio.CrearNotificacion(comando); return Ok(new { mensaje = "Notificación creada exitosamente" }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpGet("{carnetUsuario}")]
    public IActionResult ObtenerPorUsuario(string carnetUsuario)
    {
        try { var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnetUsuario); return Ok(servicio.ObtenerNotificacionesPorUsuario(consulta)); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        try { servicio.EliminarNotificacion(new EliminarNotificacionComando(id)); return NoContent(); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }

    [HttpPost("{id}/leida")]
    public IActionResult MarcarComoLeida(string id)
    {
        try { servicio.MarcarNotificacionComoLeida(new MarcarComoLeidoComando(id)); return Ok(new { mensaje = "Notificación marcada como leída" }); }
        catch (Exception ex) { return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message }); }
    }
}