using System.Data;
using IMT_Reservas.Server.Application.Interfaces;

public class NotificacionService : INotificacionService
{
    private readonly INotificacionRepository _notificacionRepository;
    private readonly IPrestamoRepository _prestamoRepository;
    public NotificacionService(INotificacionRepository notificacionRepository, IPrestamoRepository prestamoRepository)
    {
        _notificacionRepository = notificacionRepository;
        _prestamoRepository = prestamoRepository;
    }

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
                FechaEnvio = (System.DateTime)fila["fecha_envio"],
                Leido = fila.Table.Columns.Contains("leido") ? Convert.ToBoolean(fila["leido"]) : false
            });
        }
        return notificaciones;
    }

    public void MarcarNotificacionComoLeida(MarcarComoLeidoComando comando)
    {
        _notificacionRepository.MarcarComoLeida(comando);
    }

    public bool TieneNotificacionesNoLeidas(TieneNotificacionesNoLeidasConsulta consulta)
    {
        return _notificacionRepository.TieneNotificacionesNoLeidas(consulta);
    }

    private bool NotificacionYaExiste(string carnet, string titulo, string contenido)
    {
        var consulta = new ObtenerNotificacionPorCarnetUsuarioConsulta(carnet);
        var notificaciones = _notificacionRepository.ObtenerPorUsuario(consulta);
        foreach (DataRow fila in notificaciones.Rows)
        {
            if (fila["titulo"].ToString() == titulo && fila["contenido"].ToString() == contenido)
                return true;
        }
        return false;
    }

    public void EnviarNotificacionesRetraso()
    {
        var prestamos = _prestamoRepository.ObtenerTodos();
        var ahora = DateTime.UtcNow;
        foreach (DataRow fila in prestamos.Rows)
        {
            var tieneEstadoEliminado = fila.Table.Columns.Contains("estado_eliminado");
            if (fila["fecha_devolucion_esperada"] == DBNull.Value || fila["fecha_devolucion"] != DBNull.Value || (tieneEstadoEliminado && Convert.ToBoolean(fila["estado_eliminado"]))) continue;
            var fechaDevolucionEsperada = ((DateTime)fila["fecha_devolucion_esperada"]).ToUniversalTime();
            if (fechaDevolucionEsperada < ahora)
            {
                var idPrestamo = fila["id_prestamo"].ToString();
                var carnet = fila["carnet"].ToString();
                var fechaPenalizacion = fechaDevolucionEsperada.AddDays(1);
                var contenido = $"El préstamo con ID {idPrestamo} no ha sido devuelto. Si no lo entregas antes del {fechaPenalizacion:yyyy-MM-dd HH:mm}, tu cuenta será bloqueada.";
                var titulo = "Préstamo retrasado";
                if (!NotificacionYaExiste(carnet, titulo, contenido))
                {
                    var comando = new CrearNotificacionComando(carnet, titulo, contenido);
                    _notificacionRepository.Crear(comando);
                }
            }
        }
    }

    public void EnviarPenalizaciones()
    {
        var prestamos = _prestamoRepository.ObtenerTodos();
        var ahora = DateTime.UtcNow;
        foreach (DataRow fila in prestamos.Rows)
        {
            var tieneEstadoEliminado = fila.Table.Columns.Contains("estado_eliminado");
            if (fila["fecha_devolucion_esperada"] == DBNull.Value || fila["fecha_devolucion"] != DBNull.Value || (tieneEstadoEliminado && Convert.ToBoolean(fila["estado_eliminado"]))) continue;
            var fechaDevolucionEsperada = ((DateTime)fila["fecha_devolucion_esperada"]).ToUniversalTime();
            if (ahora >= fechaDevolucionEsperada.AddDays(1))
            {
                var idPrestamo = fila["id_prestamo"].ToString();
                var carnet = fila["carnet"].ToString();
                var contenido = $"El préstamo con ID {idPrestamo} no ha sido devuelto. Tu cuenta permanecerá bloqueada hasta las {fechaDevolucionEsperada.AddDays(1)}.";
                var titulo = "Cuenta bloqueada";
                if (!NotificacionYaExiste(carnet, titulo, contenido))
                {
                    var comando = new CrearNotificacionComando(carnet, titulo, contenido);
                    _notificacionRepository.Crear(comando);
                }
            }
        }
    }

    public void EnviarEstadoDelPrestamo()
    {
        var prestamos = _prestamoRepository.ObtenerTodos();
        foreach (DataRow fila in prestamos.Rows)
        {
            if (fila["estado_prestamo"] == DBNull.Value || fila["carnet"] == DBNull.Value) continue;
            var estado = fila["estado_prestamo"].ToString();
            var idPrestamo = fila["id_prestamo"].ToString();
            var carnet = fila["carnet"].ToString();
            if (estado == "aprobado")
            {
                var contenido = $"Tu solicitud de préstamo con el id {idPrestamo} ha sido aprobada.";
                var titulo = "Solicitud aprobada";
                if (!NotificacionYaExiste(carnet, titulo, contenido))
                {
                    var comando = new CrearNotificacionComando(carnet, titulo, contenido);
                    _notificacionRepository.Crear(comando);
                }
            }
            else if (estado == "rechazado")
            {
                var contenido = $"Tu solicitud de préstamo con el id {idPrestamo} ha sido rechazada.";
                var titulo = "Solicitud rechazada";
                if (!NotificacionYaExiste(carnet, titulo, contenido))
                {
                    var comando = new CrearNotificacionComando(carnet, titulo, contenido);
                    _notificacionRepository.Crear(comando);
                }
            }
        }
    }
}
