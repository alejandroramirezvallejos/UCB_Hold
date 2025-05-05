public record CrearUsuarioComando
(
    string  Carnet,
    string  Nombre,
    string  ApellidoPaterno,
    string  ApellidoMaterno,
    string  Rol,
    int     CarreraId,
    string  Contrasena,
    string  Email,
    string  Telefono,
    string? NombreReferencia,
    string? TelefonoReferencia,
    string? EmailReferencia
);
