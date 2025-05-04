public class GrupoEquipo : IGrupoEquipo, IEliminacionLogica
{
    private int     _id;
    private string  _nombre        = string.Empty;
    private string  _modelo        = string.Empty;
    private string? _urlData       = null;
    private string  _urlImagen     = string.Empty;
    private int     _cantidad;
    private string  _marca         = string.Empty;
    private int     _categoriaId;
    private bool    _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del grupo de equipo");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del grupo de equipo");
    }

    public string Modelo
    {
        get => _modelo;
        private set => _modelo = Verificar.SiEsVacio(value, "El modelo del grupo de equipo");
    }

    public string? UrlData
    {
        get => _urlData;
        private set => _urlData = value is not null
                       ? Verificar.SiEsVacio(value, "La URL del grupo de equipo")
                       : null;
    }

    public string UrlImagen
    {
        get => _urlImagen;
        private set => _urlImagen = Verificar.SiEsNulo(value, "La URL de la imagen del grupo de equipo");
    }

    public int Cantidad
    {
        get => _cantidad;
        private set => _cantidad = Verificar.SiEsNatural(value, "La cantidad de equipos");
    }

    public string Marca
    {
        get => _marca;
        private set => _marca = Verificar.SiEsVacio(value, "La marca del grupo de equipo");
    }

    public int CategoriaId
    {
        get => _categoriaId;
        private set => _categoriaId = Verificar.SiEsNatural(value, "El ID de la categoria");
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public GrupoEquipo(string nombre, string modelo, string? urlData, string urlImagen, 
                       int cantidad, string marca, int categoriaId)
    {
        Nombre      = nombre;
        Modelo      = modelo;
        UrlData     = urlData;
        UrlImagen   = urlImagen;
        Cantidad    = cantidad;
        Marca       = marca;
        CategoriaId = categoriaId;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}