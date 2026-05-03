namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoRequest
{
    public DateTime? FechaMantenimiento { get; set; }
    public DateTime? FechaFinalMantenimiento { get; set; }
    public string? TipoMantenimiento { get; set; }
    public string? Descripcion { get; set; }
    public int? IdEmpresaMantenimiento { get; set; }
}
