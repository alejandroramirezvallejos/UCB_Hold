namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoDetail
{
    public int Id { get; set; }
    public DateTime? FechaMantenimiento { get; set; }
    public DateTime? FechaFinalDeMantenimiento { get; set; }
    public int? IdEmpresa { get; set; }
    public decimal? Costo { get; set; }
    public string? Descripcion { get; set; }
    public bool EstadoEliminado { get; set; }
}
