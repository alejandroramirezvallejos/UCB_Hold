public class Mueble : IDimensiones
{
    private int     _id;
    private string  _nombre;
    private string  _tipo;
    private string  _ubicacion;
    private int     _numeroGaveteros;
    private decimal _costo;
    private int     _gaveteroId;
    private bool    _estaEliminado;
    private double  _alto;
    private double  _ancho;
    private double  _largo;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del mueble debe ser un numero natural: '{value}'",
                          nameof(value));
            _id = value;
        }
    }

    public string Nombre
    {
        get => _nombre;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El nombre del mueble no puede estar vacio",
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public string Tipo
    {
        get => _tipo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El tipo de mueble no puede estar vacio",
                          nameof(value));
            _tipo = value.Trim();
        }
    }

    public string Ubicacion
    {
        get => _ubicacion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La ubicacion del mueble no puede estar vacia",
                          nameof(value));
            _ubicacion = value.Trim();
        }
    }

    public int NumeroGaveteros
    {
        get => _numeroGaveteros;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El numero de gaveteros debe ser un numero natural: '{value}'",
                          nameof(value));
            _numeroGaveteros = value;
        }
    }

    public decimal Costo
    {
        get => _costo;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El costo debe ser un numero positivo: '{value}'",
                          nameof(value));
            _costo = value;
        }
    }

    public int GaveteroId
    {
        get => _gaveteroId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del gavetero debe ser un numero natural: '{value}'",
                          nameof(value));
            _gaveteroId = value;
        }
    }

    public Gavetero Gavetero { get; private set; }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Mueble(int id, string nombre, string tipo, string ubicacion,
                  double alto, double ancho, double largo,
                  int numeroGaveteros, decimal costo, int gaveteroId)
    {
        Id              = id;
        Nombre          = nombre;
        Tipo            = tipo;
        Ubicacion       = ubicacion;
        NumeroGaveteros = numeroGaveteros;
        Costo           = costo;
        GaveteroId      = gaveteroId;
        EstaEliminado   = false;
        SetDimensiones(alto, ancho, largo);
    }

    public void SetDimensiones(double alto, double ancho, double largo)
    {
        if (alto <= 0)
            throw new ArgumentException($"El alto debe ser un numero positivo: '{alto}'",
                      nameof(alto));
        if (ancho <= 0)
            throw new ArgumentException($"El ancho debe ser un numero positivo: '{ancho}'",
                      nameof(ancho));
        if (largo <= 0)
            throw new ArgumentException($"El largo debe ser un numero positivo: '{largo}'",
                      nameof(largo));

        _alto  = alto;
        _ancho = ancho;
        _largo = largo;
    }

    public (double alto, double ancho, double largo) GetDimensiones()
        => (_alto, _ancho, _largo);
}
