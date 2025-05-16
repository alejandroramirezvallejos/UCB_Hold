public record CrearEquipoComando
(
    int     GrupoEquipoId,
    string  CodigoImt,
    string? CodigoUcb,
    string? Descripcion,
    string  EstadoEquipo,
    string? NumeroSerial,
    string? Ubicacion,
    double? CostoReferencia,
    int?    TiempoMaximoPrestamo,
    string? Procedencia,
    int?    GaveteroId
);