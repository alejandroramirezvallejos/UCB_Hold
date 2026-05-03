namespace IMT_Reservas.Server.Application.Features.Usuario.Dtos;

// Create/Update Request DTO for Usuario


public class UsuarioDto
{
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Email { get; set; }
    public string? Contrasena { get; set; }
    public string? Rol { get; set; }
    public int? IdCarrera { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoReferencia { get; set; }
    public string? NombreReferencia { get; set; }
    public string? EmailReferencia { get; set; }
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
