namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoListDto
{
    public int Id { get; set; }
    public DateTime? FechaMantenimiento { get; set; }
    public DateTime? FechaFinalDeMantenimiento { get; set; }
    public int? IdEmpresa { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Costo { get; set; }
    public string? NombreEmpresaMantenimiento { get; set; }
    public string? TipoMantenimiento { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public int? CodigoImtEquipo { get; set; }
    public string? DescripcionEquipo { get; set; }
}

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

