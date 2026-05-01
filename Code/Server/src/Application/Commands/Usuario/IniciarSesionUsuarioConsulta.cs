namespace IMT_Reservas.Server.Application.Commands.Usuario;

public record IniciarSesionUsuarioConsulta(
    string Email,
    string Contrasena
);