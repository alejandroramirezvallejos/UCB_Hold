using System.Data;
using Ardalis.Result;

public interface INotificacionRepository
{
    Result<NotificacionDto?> Crear(CrearNotificacionComando comando);
    Result<NotificacionDto?> Eliminar(EliminarNotificacionComando comando);
    DataTable ObtenerPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta);
    void MarcarComoLeida(MarcarComoLeidoComando comando);
    bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta);
}
