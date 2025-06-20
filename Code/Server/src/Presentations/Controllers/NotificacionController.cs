using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpPost]
        public IActionResult CrearNotificacion([FromBody] CrearNotificacionComando comando)
        {
            try
            {
                _notificacionService.CrearNotificacion(comando);
                return Ok(new { message = "Notificación creada exitosamente" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpGet("{carnetUsuario}")]
        public ActionResult<List<NotificacionDto>> ObtenerNotificacionesPorUsuario(string carnetUsuario)
        {
            try
            {
                var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnetUsuario);
                var notificaciones = _notificacionService.ObtenerNotificacionesPorUsuario(consulta);
                return Ok(notificaciones);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult EliminarNotificacion(string id)
        {
            try
            {
                var comando = new EliminarNotificacionComando(id);
                _notificacionService.EliminarNotificacion(comando);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }

        [HttpPost("{id}/leida")]
        public IActionResult MarcarComoLeida(string id)
        {
            try
            {
                var comando = new MarcarComoLeidoComando(id);
                _notificacionService.MarcarNotificacionComoLeida(comando);
                return Ok(new { message = "Notificación marcada como leída" });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", message = ex.Message });
            }
        }
    }
}