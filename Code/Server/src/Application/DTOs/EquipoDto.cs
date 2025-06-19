public class EquipoDto
{
    public int     Id                   { get; set; }
    public string? NombreGrupoEquipo    { get; set; } = null;
    public int?    CodigoImt            { get; set; } = null;
    public string? CodigoUcb            { get; set; } = null;
    public string? NumeroSerial         { get; set; } = null;
    public string? EstadoEquipo         { get; set; } = null;
    public string? Ubicacion            { get; set; } = null;
    public string? NombreGavetero       { get; set; } = null;
    public double? CostoReferencia      { get; set; } = null;
    public string? Descripcion          { get; set; } = null;
    public int?    TiempoMaximoPrestamo { get; set; } = null;
    public string? Procedencia          { get; set; } = null;
}