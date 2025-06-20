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
            _notificacionRepository.Crear(comando);
        }

        public virtual void EliminarNotificacion(EliminarNotificacionComando comando)
        {
            _notificacionRepository.Eliminar(comando);
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
            _notificacionRepository.MarcarComoLeida(comando);
        }
    }
}
