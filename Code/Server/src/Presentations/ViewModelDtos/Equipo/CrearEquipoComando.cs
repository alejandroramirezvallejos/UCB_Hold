public record CrearEquipoComando
(
    string?  NombreGrupoEquipo,//Se valida si no es nulo
    string?  Modelo,//Se valida si no es nulo
    string?  Marca,//Se valida si no es nulo
    string? CodigoUcb,
    string? Descripcion,
    string? NumeroSerial,
    string? Ubicacion,
    string? Procedencia,
    double? CostoReferencia,
    int?    TiempoMaximoPrestamo,
    string? NombreGavetero
);