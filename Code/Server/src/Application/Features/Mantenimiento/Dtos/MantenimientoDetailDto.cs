namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoDetailDto
{
    public int Id { get; set; }
    public int IdEquipo { get; set; }
    public int IdEmpresaMantenimiento { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Costo { get; set; }
    public bool EstadoEliminado { get; set; }
}
