using Ardalis.Result;

public interface INotificacionService
{
    Result<NotificacionDto> Crear(CrearNotificacionComando comando);
    Result<NotificacionDto> Eliminar(EliminarNotificacionComando comando);
    List<NotificacionDto> ObtenerNotificacionesPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta);
    void MarcarNotificacionComoLeida(MarcarComoLeidoComando comando);
    bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta);
    void EnviarNotificacionesRetraso();
    void EnviarPenalizaciones();
    void EnviarEstadoDelPrestamo();
}
