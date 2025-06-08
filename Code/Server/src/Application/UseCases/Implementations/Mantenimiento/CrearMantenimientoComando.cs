//TODO: Implementar
public record CrearMantenimientoComando(
    DateOnly              FechaMantenimiento,
    DateOnly              FechaFinalDeMantenimiento,
    string                NombreEmpresaMantenimiento,
    double?               Costo,
    string?               DescripcionMantenimiento,
    int[]                 CodigoIMT,
    TipoDeMantenimiento[] TipoMantenimiento,
    string[]?             DescripcionEquipo
);