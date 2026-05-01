namespace IMT_Reservas.Server.Application.Commands.Comentario;

public record CrearComentarioComando(
    string CarnetUsuario,
    int IdGrupoEquipo,
    string Contenido
);
