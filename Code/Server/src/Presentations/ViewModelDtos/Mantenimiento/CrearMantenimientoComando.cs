public record CrearMantenimientoComando(
    DateOnly              FechaMantenimiento,
    DateOnly              FechaFinalDeMantenimiento,
    string                NombreEmpresaMantenimiento,
    double?               Costo,
    string?               DescripcionMantenimiento,
    int[]                 CodigoIMT,
    string[]              TipoMantenimiento,
    string?[]?            DescripcionEquipo
);