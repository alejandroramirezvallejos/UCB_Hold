public record EquipoCreateDto
{
    public string   CodigoEquipo         { get; init; }
    public string   CodigoImt            { get; init; }
    public string   CodigoUcb            { get; init; }
    public string   Descripcion          { get; init; }
    public string   EstadoTecnico        { get; init; }
    public string   NumeroSerial         { get; init; }
    public string   Ubicacion            { get; init; }
    public decimal? CostoReferencia      { get; init; }
    public int?     TiempoMaximoPrestamo { get; init; }
    public string   Procedencia          { get; init; }
}
