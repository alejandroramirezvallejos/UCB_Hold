public record CrearEquipoComando
(
    string  NombreGrupoEquipo,
    string  Modelo,
    string  Marca,
    string? CodigoUcb,
    string? Descripcion,
    string? NumeroSerial,
    string? Ubicacion,
    string? Procedencia,
    double? CostoReferencia,
    int?    TiempoMaximoPrestamo,
    string? NombreGavetero
);