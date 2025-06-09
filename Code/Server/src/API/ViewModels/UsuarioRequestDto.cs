namespace API.ViewModels;

public class UsuarioRequestDto
{
    public string? Carnet              { get; set; } = null;
    public string? Nombre              { get; set; } = null;
    public string? ApellidoPaterno     { get; set; } = null;
    public string? ApellidoMaterno     { get; set; } = null;
    public string? Email               { get; set; } = null;
    public string? Contrasena          { get; set; } = null;
    public string? NombreCarrera       { get; set; } = null;
    public string? Telefono            { get; set; } = null;
    public string? TelefonoReferencia  { get; set; } = null;
    public string? NombreReferencia    { get; set; } = null;
    public string? EmailReferencia     { get; set; } = null;
}
