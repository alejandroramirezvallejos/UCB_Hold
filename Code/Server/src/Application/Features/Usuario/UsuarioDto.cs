namespace IMT_Reservas.Server.Application.Features.Usuario.Dtos;

public class UsuarioDto
{
    public int? Id { get; set; }
    public string? Carnet { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Rol { get; set; }
    public string? Carrera { get; set; }
    public string? Correo { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Telefono { get; set; }
    public string? NombreReferencia { get; set; }
    public string? TelefonoReferencia { get; set; }
    public string? EmailReferencia { get; set; }
}
