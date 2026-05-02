namespace IMT_Reservas.Server.Application.Features.Usuario.Dtos;

public class UsuarioListDto
{
	public int Id { get; set; }
	public string? Carnet { get; set; }
	public string? Nombre { get; set; }
	public string? Email { get; set; }
	public string? Rol { get; set; }
	public int? IdCarrera { get; set; }
}
