public class Accesorio : IAccesorio, IEliminacionLogica
{
    private int     _id;
    private string  _nombre        = string.Empty;
    private string? _descripcion   = null;
    private string? _modelo        = null;
    private string? _url           = null;
    private double? _precio        = null;
    private int     _equipoId;
    private string? _tipo          = null;
    private bool    _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del accesorio");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del accesorio");
    }

    public string? Descripcion
    {
        get => _descripcion;
        private set => _descripcion = value is not null
                       ? Verificar.SiEsVacio(value, "La descripcion del accesorio")
                       : null;
    }

    public string? Modelo
    {
        get => _modelo;
        private set => _modelo = value is not null
                       ? Verificar.SiEsVacio(value, "El modelo del accesorio")
                       : null;
    }

    public string? Url
    {
        get => _url;
        private set => _url = value is not null
                       ? Verificar.SiEsVacio(value, "La URL del accesorio")
                       : null;
    }

    public double? Precio
    {
        get => _precio;
        private set => _precio = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El precio del accesorio")
                       : null;
    }

    public int EquipoId
    {
        get => _equipoId;
        private set => _equipoId = Verificar.SiEsNatural(value, "El ID del equipo");
    }

    public string? Tipo
    {
        get => _tipo;
        private set
        {
            if (value is not null)
            {
                Enum enumTipoDeAccesorio = Verificar.SiEstaEnEnum<TipoDeAccesorio>(value, "El tipo de accesorio");
                _tipo = enumTipoDeAccesorio.ToString();
            }
            else
            {
                _tipo = null;
            }
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Accesorio(string nombre, string? descripcion, string? modelo,
                     string? url, double? precio, int equipoId, string? tipo)
    {
        Nombre      = nombre;
        Descripcion = descripcion;
        Modelo      = modelo;
        Url         = url;
        Precio      = precio;
        EquipoId    = equipoId;
        Tipo        = tipo;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}



