public class Gavetero : IDimensiones
{
    private int    _id;
    private string _nombre;
    private string _tipo;
    private bool   _estaEliminado;
    private double _alto;
    private double _ancho;
    private double _largo;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del gavetero debe ser un numero natural: '{value}'",
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
                throw new ArgumentException("El nombre del gavetero no puede estar vacio",
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
                throw new ArgumentException("El tipo de gavetero no puede estar vacio", 
                          nameof(value));
            _tipo = value.Trim();
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Gavetero(int id, string nombre, string tipo,
                    double alto, double ancho, double largo)
    {
        Id            = id;
        Nombre        = nombre;
        Tipo          = tipo;
        EstaEliminado = false;
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
