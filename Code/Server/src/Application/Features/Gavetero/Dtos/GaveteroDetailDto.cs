namespace IMT_Reservas.Server.Application.Features.Gavetero.Dtos;

public class GaveteroDetailDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public int IdMueble { get; set; }
	public bool EstadoEliminado { get; set; }
}
