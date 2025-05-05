public class UsuarioResponseDto
{
    public string  Carnet             { get; set; } = string.Empty;
    public string  Nombre             { get; set; } = string.Empty;
    public string  ApellidoPaterno    { get; set; } = string.Empty;
    public string  ApellidoMaterno    { get; set; } = string.Empty;
    public string  Rol                { get; set; } = string.Empty;
    public int     CarreraId          { get; set; }
    public string  Email              { get; set; } = string.Empty;
    public string  Telefono           { get; set; } = string.Empty;
    public string? NombreReferencia   { get; set; } = null;
    public string? TelefonoReferencia { get; set; } = null;
    public string? EmailReferencia    { get; set; } = null;
    public bool    EstaEliminado      { get; set; } = false;
}