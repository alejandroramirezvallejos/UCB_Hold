namespace API.ViewModels;

public class MantenimientoRequestDto
{
    public DateOnly? FechaMantenimiento { get; set; } = null;
    public DateOnly? FechaFinalDeMantenimiento { get; set; } = null;
    public string? NombreEmpresaMantenimiento { get; set; } = null;
    public double? Costo { get; set; } = null;
    public string? DescripcionMantenimiento { get; set; } = null;
    public int[]? CodigoIMT { get; set; } = null;
    public string[]? TipoMantenimiento { get; set; } = null;
    public string?[]? DescripcionEquipo { get; set; } = null;
}