public interface IComponente
{
    int     Id               { get; }
    string  Nombre           { get; }
    string? Descripcion      { get; }
    string? Modelo           { get; }
    string? Url              { get; }
    string? Tipo             { get; }
    double? PrecioReferencia { get; }
    int     EquipoId         { get; }
}
