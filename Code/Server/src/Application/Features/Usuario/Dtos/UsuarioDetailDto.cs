namespace IMT_Reservas.Server.Application.Features.Usuario.Dtos;

public class UsuarioDetailDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public string? Email { get; set; }
	public string? Contrasena { get; set; }
	public int? IdCarrera { get; set; }
	public string? Rol { get; set; }
	public bool EstadoEliminado { get; set; }
}
