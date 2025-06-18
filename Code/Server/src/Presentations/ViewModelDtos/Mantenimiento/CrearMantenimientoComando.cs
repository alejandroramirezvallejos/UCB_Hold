public record CrearMantenimientoComando(
    DateOnly?              FechaMantenimiento,//Se valida si no es nulo
    DateOnly?              FechaFinalDeMantenimiento,//Se valida si no es nulo
    string?                NombreEmpresaMantenimiento,//Se valida si no es nulo
    double?               Costo,
    string?               DescripcionMantenimiento,
    int[]?                CodigoIMT,//Se valida si no es nulo - CAMBIADO: de int?[]? a int[]?
    string?[]?            TipoMantenimiento,//Se valida si no es nulo
    string?[]?            DescripcionEquipo
);