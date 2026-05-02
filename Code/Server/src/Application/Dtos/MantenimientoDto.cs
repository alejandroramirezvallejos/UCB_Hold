namespace IMT_Reservas.Server.Application.Dtos;

public class MantenimientoDto
{
    public string? Descripcion { get; set; }
    public decimal? Costo { get; set; }
    public DateTime? FechaMantenimiento { get; set; }
    public int? IdEmpresa { get; set; }
    public DateTime? FechaFinalMantenimiento { get; set; }
}
