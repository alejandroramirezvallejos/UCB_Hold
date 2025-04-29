public class Componente
{
    private int     _id;
    private string  _nombre;
    private string? _descripcion = null;
    private string? _modelo = null;
    private string? _url = null;
    private string? _tipo = null;
    private double? _precioReferencia = null;
    private int     _equipoId;
    private bool    _estaEliminado = false;
     
    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del componente");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del componente");
    }

    public string? Descripcion
    {
        get => _descripcion;
        private set => _descripcion = value is not null
                       ? Verificar.SiEsVacio(value, "La descripción del componente")
                       : null;
    }

    public string? Modelo
    {
        get => _modelo;
        private set => _modelo = value is not null
                       ? Verificar.SiEsVacio(value, "El modelo del componente")
                       : null;
    }

    public string? Url
    {
        get => _url;
        private set => _url = value is not null
                       ? Verificar.SiEsNulo(value, "La URL del componente")
                       : null;
    }

    public string? Tipo
    {
        get => _tipo;
        private set
        {
            if (value is not null)
            {
                Enum enumTipoDeComponente = Verificar.SiEstaEnEnum<TipoDeComponente>(value, "El tipo de componente");
                _tipo = enumTipoDeComponente.ToString();
            }
            else
            {
                _tipo = null;
            }
        }
    }

    public double? PrecioReferencia
    {
        get => _precioReferencia;
        private set => _precioReferencia = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El precio de referencia del componente")
                       : null;
    }

    public int EquipoId
    {
        get => _equipoId;
        private set => _equipoId = Verificar.SiEsNatural(value, "El ID del equipo");
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Componente(string nombre, string? descripcion, string? modelo,
        string? url, string? tipo, double? precioReferencia, int equipoId)
    {
        Nombre           = nombre;
        Descripcion      = descripcion;
        Modelo           = modelo;
        Url              = url;
        Tipo             = tipo;
        PrecioReferencia = precioReferencia;
        EquipoId         = equipoId;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
