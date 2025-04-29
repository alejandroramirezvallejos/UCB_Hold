public class Gavetero : IDimensiones
{
    private int     _id;
    private string  _nombre;
    private string? _tipo = null;
    private bool    _estaEliminado = false;
    private int     _muebleId;
    private double? _alto = null;
    private double? _ancho = null;
    private double? _largo = null;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del gavetero");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del gavetero");
    }

    public string? Tipo
    {
        get => _tipo;
        private set
        {
            if (value is not null)
            {
                Enum enumTipoDeGavetero = Verificar.SiEstaEnEnum<TipoDeGavetero>(value, "El tipo de gavetero");
                _tipo = enumTipoDeGavetero.ToString();
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

    public int MuebleId
    {
        get => _muebleId;
        private set => _muebleId = Verificar.SiEsNatural(value, "El ID del mueble");
    }

    public double? Alto
    {
        get => _alto;
        private set => _alto = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El alto del gavetero")
                       : null;
    }

    public double? Ancho
    {
        get => _ancho;
        private set => _ancho = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El ancho del gavetero")
                       : null;
    }

    public double? Largo
    {
        get => _largo;
        private set => _largo = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El largo del gavetero")
                       : null;
    }

    public Gavetero(string nombre, string? tipo, int muebleId,
                    double? alto, double? ancho, double? largo)
    {
        Nombre        = nombre;
        Tipo          = tipo;
        MuebleId      = muebleId;
        SetDimensiones(alto, ancho, largo);
    }

    public void SetDimensiones(double? alto, double? ancho, double? largo)
    {
        Alto  = alto;
        Ancho = ancho;
        Largo = largo;
    }

    public (double? alto, double? ancho, double? largo) GetDimensiones() => (_alto, _ancho, _largo);

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}

