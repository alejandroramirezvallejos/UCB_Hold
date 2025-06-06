//Implementar
public record CrearMantenimientoComando(
    DateOnly FechaMantenimiento,
    DateOnly FechaFinalDeMantenimiento,
    string      NombreEmpresaMantenimiento,
    double?  Costo,
    string?  Descripcion,
    int[] CodigoIMT,
    TipoDeMantenimiento[]   TipoMantenimiento,
    string[]?  DescripcionMantenimiento
);