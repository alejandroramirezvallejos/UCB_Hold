namespace IMT_Reservas.Server.Application.Features.Mueble.Dtos;

public class MuebleListDto
{
	public int Id { get; set; }
	public string? Nombre { get; set; }
	public string? Ubicacion { get; set; }
	public int? NumeroGaveteros { get; set; }
	public string? Tipo { get; set; }
	public decimal? Costo { get; set; }
	public decimal? Longitud { get; set; }
	public decimal? Profundidad { get; set; }
	public decimal? Altura { get; set; }
}
