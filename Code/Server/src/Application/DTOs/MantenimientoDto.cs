public class MantenimientoDto : BaseDto
{
    public string? NombreEmpresaMantenimiento { get; set; }
    public DateOnly? FechaMantenimiento { get; set; }
    public DateOnly? FechaFinalDeMantenimiento { get; set; }
    public double? Costo { get; set; }
    public string? Descripcion { get; set; }
    public string? TipoMantenimiento { get; set; }
    public string? NombreGrupoEquipo { get; set; }
    public int? CodigoImtEquipo { get; set; }
    public string? DescripcionEquipo { get; set; }
}