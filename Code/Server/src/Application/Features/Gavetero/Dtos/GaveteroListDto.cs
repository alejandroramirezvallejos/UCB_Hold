namespace IMT_Reservas.Server.Application.Features.Gavetero.Dtos;

public class GaveteroListDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public int IdMueble { get; set; }
	public string? Tipo { get; set; }
	public string? NombreMueble { get; set; }
	public decimal? Longitud { get; set; }
	public decimal? Profundidad { get; set; }
	public decimal? Altura { get; set; }
}
