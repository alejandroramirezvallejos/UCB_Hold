namespace IMT_Reservas.Server.Application.Features.Accesorio.Dtos;

public class AccesorioListDto
{
	public int Id { get; set; }
	public string? nombre { get; set; }
	public string? modelo { get; set; }
	public string? tipo { get; set; }
	public string? descripcion { get; set; }
	public string? codigo_imt { get; set; }
	public decimal? precio { get; set; }
	public string? url_data_sheet { get; set; }
	public string? nombreEquipoAsociado { get; set; }
}
