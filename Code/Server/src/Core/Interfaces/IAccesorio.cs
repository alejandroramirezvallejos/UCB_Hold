public interface IAccesorio
{
    int     Id          { get; }
    string  Nombre      { get; }
    string? Descripcion { get; }
    string? Modelo      { get; }
    string? Url         { get; }
    double? Precio      { get; }
    int     EquipoId    { get; }
    string? Tipo        { get; }
}
