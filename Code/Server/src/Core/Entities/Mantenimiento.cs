namespace IMT_Reservas.Server.Core.Entities;

public class Mantenimiento : Entity
{
    public int IdEquipo { get; set; }
    public int IdEmpresaMantenimiento { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Costo { get; set; }
    public bool EstadoEliminado { get; set; }
}
