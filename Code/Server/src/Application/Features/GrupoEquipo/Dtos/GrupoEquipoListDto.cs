namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;

public class GrupoEquipoListDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public string? Modelo { get; set; }
	public string? Marca { get; set; }
	public int? Cantidad { get; set; }
	public int? IdCategoria { get; set; }
}
