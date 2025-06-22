using System.Data;
using IMT_Reservas.Server.Application.Interfaces;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    public NotificacionService(INotificacionRepository notificacionRepository) => _notificacionRepository = notificacionRepository;

    public void CrearNotificacion(CrearNotificacionComando comando)
    {
        _notificacionRepository.Crear(comando);
    }

    public void EliminarNotificacion(EliminarNotificacionComando comando)
    {
        _notificacionRepository.Eliminar(comando);
    }

    public List<NotificacionDto> ObtenerNotificacionesPorUsuario(ObtenerNotificacionPorCarnetUsuarioConsulta consulta)
    {
        var tabla = _notificacionRepository.ObtenerPorUsuario(consulta);
        var notificaciones = new List<NotificacionDto>(tabla.Rows.Count);
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

    public void MarcarNotificacionComoLeida(MarcarComoLeidoComando comando)
    {
        _notificacionRepository.MarcarComoLeida(comando);
    }
}
