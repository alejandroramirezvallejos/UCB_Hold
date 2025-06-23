using System.Data;

public interface INotificacionRepository
{
    void Crear(CrearNotificacionComando comando);
    void Eliminar(EliminarNotificacionComando comando);
    DataTable ObtenerPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta);
    void MarcarComoLeida(MarcarComoLeidoComando comando);
    bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta);
}