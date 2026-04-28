using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
    private readonly NotificacionService servicio;
    public NotificacionController(NotificacionService servicio) => this.servicio = servicio;

    [HttpPost]
    public IActionResult Crear([FromBody] CrearNotificacionComando comando)
    {
        servicio.Crear(comando); return Ok(new { mensaje = "Notificación creada exitosamente" });
    }

    [HttpGet("{carnetUsuario}")]
    public IActionResult ObtenerPorUsuario(string carnetUsuario)
    {
        var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnetUsuario); return Ok(servicio.ObtenerNotificacionesPorUsuario(consulta));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        servicio.Eliminar(new EliminarNotificacionComando(id)); return NoContent();
    }

    [HttpPost("{id}/leida")]
    public IActionResult MarcarComoLeida(string id)
    {
        servicio.MarcarNotificacionComoLeida(new MarcarComoLeidoComando(id)); return Ok(new { mensaje = "Notificación marcada como leída" });
    }

    [HttpGet("{carnetUsuario}/tiene-no-leidas")]
    public IActionResult TieneNoLeidas(string carnetUsuario)
    {
        try
        {
            var consulta = new TieneNotificacionesNoLeidasConsulta(carnetUsuario);
            var tiene = servicio.TieneNotificacionesNoLeidas(consulta);
            return Ok(new { tieneNoLeidas = tiene });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.GetType().Name, mensaje = ex.Message });
        }
    }

    [HttpPost("enviar-retrasos")]
    public IActionResult EnviarRetrasos()
    {
        servicio.EnviarNotificacionesRetraso(); return Ok(new { mensaje = "Notificaciones de retraso enviadas" });
    }

    [HttpPost("enviar-penalizaciones")]
    public IActionResult EnviarPenalizaciones()
    {
        servicio.EnviarPenalizaciones(); return Ok(new { mensaje = "Penalizaciones enviadas" });
    }

    [HttpPost("enviar-estado-prestamo")]
    public IActionResult EnviarEstadoPrestamo()
    {
        servicio.EnviarEstadoDelPrestamo(); return Ok(new { mensaje = "Notificaciones de estado de préstamo enviadas" });
    }

}