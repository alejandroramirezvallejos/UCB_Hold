public record CrearUsuarioComando
(
    string?  Carnet,//Se valida si no es nulo
    string?  Nombre,//Se valida si no es nulo
    string?  ApellidoPaterno,//Se valida si no es nulo
    string?  ApellidoMaterno,//Se valida si no es nulo
    string?  Rol,
    string?  Email,//Se valida si no es nulo
    string?  Contrasena,//Se valida si no es nulo
    string?  NombreCarrera,//Se valida si no es nulo
    string?  Telefono,//Se valida si no es nulo
    string?  TelefonoReferencia,
    string?  NombreReferencia,
    string?  EmailReferencia
);
