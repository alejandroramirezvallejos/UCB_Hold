public class GaveteroDto
{
    public int     Id            { get; set; }
    public string  Nombre        { get; set; } = string.Empty;
    public string? Tipo          { get; set; } = null;
    public bool    EstaEliminado { get; set; } = false;
    public int     MuebleId      { get; set; }
    public double? Alto          { get; set; } = null;
    public double? Ancho         { get; set; } = null;
    public double? Largo         { get; set; } = null;
}