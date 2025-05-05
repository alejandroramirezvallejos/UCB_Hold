public class MantenimientoDto
{
    public int      Id                        { get; set; }
    public string   Tipo                      { get; set; } = string.Empty;
    public string?  Descripcion               { get; set; } = null;
    public double?  Costo                     { get; set; } = null;
    public DateOnly FechaMantenimiento        { get; set; }
    public DateOnly FechaFinalDeMantenimiento { get; set; }
    public int      EmpresaMantenimientoId    { get; set; }
    public bool     EstaEliminado             { get; set; } = false;
}