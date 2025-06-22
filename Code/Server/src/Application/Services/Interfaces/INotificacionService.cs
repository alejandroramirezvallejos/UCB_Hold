namespace IMT_Reservas.Server.Application.Interfaces
{
    public interface INotificacionService
    {
        void CrearNotificacion(CrearNotificacionComando comando);
        void EliminarNotificacion(EliminarNotificacionComando comando);
        List<NotificacionDto> ObtenerNotificacionesPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta);
        void MarcarNotificacionComoLeida(MarcarComoLeidoComando comando);
    }
}
