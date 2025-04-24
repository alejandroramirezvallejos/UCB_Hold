public class GrupoEquipo
{
    public int       Id            { get; private set; }
    public string    Nombre        { get; private set; }
    public string    Modelo        { get; private set; }
    public Uri       DataSheetUrl  { get; private set; }
    public int       Cantidad      { get; private set; }
    public string    Marca         { get; private set; }
    public int       CategoriaId   { get; private set; }
    public Categoria Categoria     { get; private set; }
    public bool      EstaEliminado { get; private set; }

    public GrupoEquipo(string nombre, string modelo, Uri dataSheetUrl, int cantidad, 
                       string marca, int categoriaId)
    {
        Nombre        = nombre;
        Modelo        = modelo;
        DataSheetUrl  = dataSheetUrl;
        Cantidad      = cantidad;
        Marca         = marca;
        CategoriaId   = categoriaId;
        EstaEliminado = false;
    }
}
