public interface IGrupoEquipo
{
    int     Id          { get; }
    string  Nombre      { get; }
    string  Modelo      { get; }
    string? UrlData     { get; }
    string  UrlImagen   { get; }
    int     Cantidad    { get; }
    string  Marca       { get; }
    int     CategoriaId { get; }
}
