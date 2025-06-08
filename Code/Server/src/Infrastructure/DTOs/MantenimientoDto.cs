public class MantenimientoDto
{
    public string? NombreEmpresaMantenimiento { get; set; } = null;
    public DateOnly? FechaMantenimiento { get; set; } = null;
    public DateOnly? FechaFinalDeMantenimiento { get; set; } = null;
    public double? Costo { get; set; } = null;
    public string? Descripcion { get; set; } = null;
    public string? TipoMantenimiento { get; set; } = null;
    public string? NombreGrupoEquipo { get; set; } = null;
    public int? CodigoImtEquipo { get; set; } = null;
    public string? DescripcionEquipo { get; set; } = null;
}