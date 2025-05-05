public interface IMantenimiento
{
    int      Id                        { get; }
    string   Tipo                      { get; }
    string?  Descripcion               { get; }
    double?  Costo                     { get; }
    DateOnly FechaMantenimiento        { get; }
    DateOnly FechaFinalDeMantenimiento { get; }
    int      EmpresaMantenimientoId    { get; }
}
