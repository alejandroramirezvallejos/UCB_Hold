public class DetalleMantenimientoDto
{
    public int     Id              { get; set; }
    public int     IdMantenimiento { get; set; }
    public string? Descripcion     { get; set; } = null;
    public int     IdEquipo        { get; set; }
}