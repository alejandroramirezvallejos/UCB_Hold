public interface IUsuario
{
    string  Carnet             { get; }
    string  Nombre             { get; }
    string  ApellidoPaterno    { get; }
    string  ApellidoMaterno    { get; }
    string  Rol                { get; }
    int     CarreraId          { get; }
    string  Contrasena         { get; }
    string  Email              { get; }
    string  Telefono           { get; }
    string? NombreReferencia   { get; }
    string? TelefonoReferencia { get; }
    string? EmailReferencia    { get; }
}
