public interface IEquipo
{
    int      Id                   { get; }
    int      GrupoEquipoId        { get; }
    string   CodigoImt            { get; }
    string?  CodigoUcb            { get; }
    string?  Descripcion          { get; }
    string   EstadoEquipo         { get; }
    string?  NumeroSerial         { get; }
    string?  Ubicacion            { get; }    double?  CostoReferencia      { get; }
    int?     TiempoMaximoPrestamo { get; }
    string?  Procedencia          { get; }
    string?  NombreGavetero       { get; }
    string   EstadoDisponibilidad { get; }
    DateOnly FechaDeIngreso       { get; }
}
