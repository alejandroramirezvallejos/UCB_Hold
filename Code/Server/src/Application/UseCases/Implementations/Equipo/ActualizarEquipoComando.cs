public record ActualizarEquipoComando
(
    int     Id,
    string?  NombreGrupoEquipo,
    string? CodigoUcb,
    string? Descripcion,
    string? NumeroSerial,
    string? Ubicacion,
    string? Procedencia,
    double? CostoReferencia,
    int?    TiempoMaximoPrestamo,
    string? NombreGavetero,
    string? EstadoEquipo
);