namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoListDto
{
	public int Id { get; set; }
	public DateTime? FechaMantenimiento { get; set; }
	public DateTime? FechaFinalMantenimiento { get; set; }
	public int? IdEmpresa { get; set; }
	public string? Descripcion { get; set; }
	public decimal? Costo { get; set; }
}
