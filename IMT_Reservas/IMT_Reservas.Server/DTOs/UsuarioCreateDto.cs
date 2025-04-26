public record UsuarioCreateDto
{
    public string CarnetIdentidad { get; init; }
    public string Nombre { get; init; }
    public string ApellidoPaterno { get; init; }
    public string ApellidoMaterno { get; init; }
    public string TipoUsuario { get; init; }
    public string Carrera { get; init; }
    public string Password { get; init; }
    public string Email { get; init; }
    public string Telefono { get; init; }
    public string NombreReferencia { get; init; }
    public string TelefonoReferencia { get; init; }
    public string EmailReferencia { get; init; }
}