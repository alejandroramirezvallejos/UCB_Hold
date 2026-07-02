namespace IMT_Reservas.Server.Application.Features.Usuario;

public class LoginDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}
