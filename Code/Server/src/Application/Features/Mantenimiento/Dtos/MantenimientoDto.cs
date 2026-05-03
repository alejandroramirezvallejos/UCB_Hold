namespace IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;

public class MantenimientoDto
{
    public int? Id { get; set; }
    public string? NombreEmpresaMantenimiento { get; set; }
    public DateTime? FechaMantenimiento { get; set; }
    public DateTime? FechaFinalDeMantenimiento { get; set; }
    public decimal? Costo { get; set; }
    public string? Descripcion { get; set; }
    public string? TipoMantenimiento { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public string? CodigoImtEquipo { get; set; }
    public string? DescripcionEquipo { get; set; }
}
