namespace IMT_Reservas.Server.Application.Features.Equipo.Dtos;

public class EquipoListDto
{
	public int Id { get; set; }
	public string? CodigoUcb { get; set; }
	public int? CodigoImt { get; set; }
	public string? Descripcion { get; set; }
	public string? NumeroSerial { get; set; }
	public string? Ubicacion { get; set; }
	public decimal? CostoReferencia { get; set; }
	public int? TiempoMaxPrestamo { get; set; }
	public string? Procedencia { get; set; }
	public int? IdGavetero { get; set; }
	public int IdGrupoEquipo { get; set; }
	public string? EstadoEquipo { get; set; }
}
