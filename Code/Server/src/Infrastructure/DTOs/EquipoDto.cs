public class EquipoDto
{
    public int      Id                   { get; set; }
    public int      GrupoEquipoId        { get; set; }
    public string   CodigoImt            { get; set; } = string.Empty;
    public string?  CodigoUcb            { get; set; } = null;
    public string?  Descripcion          { get; set; } = null;
    public string   EstadoEquipo         { get; set; } = string.Empty;
    public string?  NumeroSerial         { get; set; } = null;
    public string?  Ubicacion            { get; set; } = null;
    public double?  CostoReferencia      { get; set; } = null;
    public int?     TiempoMaximoPrestamo { get; set; } = null;
    public string?  Procedencia          { get; set; } = null;
    public int?     GaveteroId           { get; set; } = null;
    public string   EstadoDisponibilidad { get; set; } = string.Empty;
    public bool     EstaEliminado        { get; set; } = false;
    public DateOnly FechaDeIngreso       { get; set; }
}