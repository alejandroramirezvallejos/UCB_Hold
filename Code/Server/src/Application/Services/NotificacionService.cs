using System.Data;
using IMT_Reservas.Server.Application.Interfaces;

namespace IMT_Reservas.Server.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionService(INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        public virtual void CrearNotificacion(CrearNotificacionComando comando)
        {
            try
            {
                _notificacionRepository.Crear(comando);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error al crear la notificación", ex);
            }
        }

        public virtual void EliminarNotificacion(EliminarNotificacionComando comando)
        {
            try
            {
                _notificacionRepository.Eliminar(comando);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error al eliminar la notificación", ex);
            }
        }

        public virtual List<NotificacionDto> ObtenerNotificacionesPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta)
        {
            var tabla = _notificacionRepository.ObtenerPorUsuario(consulta);
            var notificaciones = new List<NotificacionDto>();

            foreach (DataRow fila in tabla.Rows)
            {
                notificaciones.Add(new NotificacionDto
                {
                    Id = fila["id_notificacion"].ToString(),
                    CarnetUsuario = fila["carnet_usuario"].ToString(),
                    Titulo = fila["titulo"].ToString(),
                    Contenido = fila["contenido"].ToString(),
                    FechaEnvio = (System.DateTime)fila["fecha_envio"]
                });
            }

            return notificaciones;
        }

        public virtual void MarcarNotificacionComoLeida(MarcarComoLeidoComando comando)
        {
            try
            {
                _notificacionRepository.MarcarComoLeida(comando);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception("Error al marcar la notificación como leída", ex);
            }
        }
    }
}
