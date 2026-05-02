namespace IMT_Reservas.Server.Application.Features.Accesorio.Dtos;

public class AccesorioListDto
{
	public int Id { get; set; }
	public string? Tipo { get; set; }
	public string? Nombre { get; set; }
	public string? Modelo { get; set; }
	public decimal? Precio { get; set; }
}
