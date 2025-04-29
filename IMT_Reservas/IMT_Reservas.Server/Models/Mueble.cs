public class Mueble : IDimensiones
{
    private int     _id;
    private string  _nombre;
    private string? _tipo = null;
    private string? _ubicacion = null;
    private double? _numeroGaveteros = null;
    private double? _costo = null;
    private bool    _estaEliminado = false;
    private double? _alto = null;
    private double? _ancho = null;
    private double? _largo = null;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del mueble");
    }

    public string Nombre
    {
        get => _nombre;
        private set => _nombre = Verificar.SiEsVacio(value, "El nombre del mueble");
    }

    public string? Tipo
    {
        get => _tipo;
        private set
        {
            if (value is not null)
            {
                Enum enumTipoDeMueble = Verificar.SiEstaEnEnum<TipoDeMueble>(value, "El tipo de mueble");
                _tipo = enumTipoDeMueble.ToString();
            }
            else
            {
                _tipo = null;
            }
        }
    }

    public string? Ubicacion
    {
        get => _ubicacion;
        private set => _ubicacion = value is not null
                       ? Verificar.SiEsVacio(value, "La ubicacion del mueble")
                       : null;
    }

    public double? NumeroGaveteros
    {
        get => _numeroGaveteros;
        private set => _numeroGaveteros = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El numero de gaveteros")
                       : null;
    }
    public double? Costo
    {
        get => _costo;
        private set => _costo = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El costo del mueble")
                       : null;
    }

    public double? Alto
    {
        get => _alto;
        private set => _alto = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El alto del mueble")
                       : null;
    }

    public double? Ancho
    {
        get => _ancho;
        private set => _ancho = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El ancho del mueble")
                       : null;
    }

    public double? Largo
    {
        get => _largo;
        private set => _largo = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El largo del mueble")
                       : null;
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Mueble(string nombre, string? tipo, string? ubicacion,
                  double? alto, double? ancho, double? largo,
                  double? numeroGaveteros, double? costo)
    {
        Nombre          = nombre;
        Tipo            = tipo;
        Ubicacion       = ubicacion;
        NumeroGaveteros = numeroGaveteros;
        Costo           = costo;
        SetDimensiones(alto, ancho, largo);
    }

    public void SetDimensiones(double? alto, double? ancho, double? largo)
    {
        Alto = alto;
        Ancho = ancho;
        Largo = largo;
    }

    public (double? alto, double? ancho, double? largo) GetDimensiones() => (_alto, _ancho, _largo);

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
