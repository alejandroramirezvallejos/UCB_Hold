namespace IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;

public class GrupoEquipoListDto
{
	public int id { get; set; }
	public string? nombre { get; set; }
	public string? modelo { get; set; }
	public string? marca { get; set; }
	public int? Cantidad { get; set; }
	public int? IdCategoria { get; set; }
	public string? descripcion { get; set; }
	public string? url_data_sheet { get; set; }
	public string? link { get; set; }
	public string? nombreCategoria { get; set; }
	public decimal? CostoPromedio { get; set; }
}
