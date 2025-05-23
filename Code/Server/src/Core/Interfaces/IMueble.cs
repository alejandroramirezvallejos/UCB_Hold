public interface IMueble
{
    int     Id              { get; }
    string  Nombre          { get; }
    string? Tipo            { get; }
    string? Ubicacion       { get; }
    double? NumeroGaveteros { get; }
    double? Costo           { get; }
}
