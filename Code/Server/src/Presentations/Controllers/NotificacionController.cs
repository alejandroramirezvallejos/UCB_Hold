using Microsoft.AspNetCore.Mvc;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class NotificacionController : ControllerBase
{
    private readonly NotificacionService _servicio;
    public NotificacionController(NotificacionService servicio) => _servicio = servicio;

    [HttpPost]
    public NotificacionDto Crear([FromBody] CrearNotificacionComando comando)
    {
        return _servicio.Crear(comando);
    }

    [HttpGet("{carnetUsuario}")]
    public IActionResult ObtenerPorUsuario(string carnetUsuario)
    {
        var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnetUsuario);
        return Ok(_servicio.ObtenerNotificacionesPorUsuario(consulta));
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(string id)
    {
        _servicio.Eliminar(new EliminarNotificacionComando(id));
        return NoContent();
    }

    [HttpPost("{id}/leida")]
    public IActionResult MarcarComoLeida(string id)
    {
        _servicio.MarcarNotificacionComoLeida(new MarcarComoLeidoComando(id));
        return Ok(new { mensaje = "Notificación marcada como leída" });
    }

    [HttpGet("{carnetUsuario}/tiene-no-leidas")]
    public IActionResult TieneNoLeidas(string carnetUsuario)
    {
        try
        {
            var consulta = new TieneNotificacionesNoLeidasConsulta(carnetUsuario);
            var tiene = _servicio.TieneNotificacionesNoLeidas(consulta);
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
        _servicio.EnviarNotificacionesRetraso();
        return Ok(new { mensaje = "Notificaciones de retraso enviadas" });
    }

    [HttpPost("enviar-penalizaciones")]
    public IActionResult EnviarPenalizaciones()
    {
        _servicio.EnviarPenalizaciones();
        return Ok(new { mensaje = "Penalizaciones enviadas" });
    }

    [HttpPost("enviar-estado-prestamo")]
    public IActionResult EnviarEstadoPrestamo()
    {
        _servicio.EnviarEstadoDelPrestamo();
        return Ok(new { mensaje = "Notificaciones de estado de préstamo enviadas" });
    }
}
