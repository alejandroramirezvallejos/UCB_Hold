namespace IMT_Reservas.Server.Application.Commands.Notificacion;

public record CrearNotificacionComando(
    string CarnetUsuario,
    string Titulo,
    string Contenido
);
