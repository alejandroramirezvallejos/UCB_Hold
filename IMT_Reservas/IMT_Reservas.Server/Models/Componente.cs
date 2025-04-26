public class Componente
{
    private int     _id;
    private string  _nombre;
    private string  _descripcion;
    private string  _modelo;
    private Uri     _dataSheetUrl;
    private string  _tipo;
    private double _precioReferencia;
    private int     _equipoId;
    private Equipo  _equipo;
    private bool    _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de componente debe ser un numero natural: '{value}'",
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
                throw new ArgumentException("El nombre del componente no puede estar vacio",
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El descripcion del componente no puede estar vacia",
                          nameof(value));
            _descripcion = value.Trim();
        }
    }

    public string Modelo
    {
        get => _modelo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El modelo del componente no puede estar vacio",
                          nameof(value));
            _modelo = value.Trim();
        }
    }

    public Uri DataSheetUrl
    {
        get => _dataSheetUrl;
        private set
        {
            if (value == null)
                throw new ArgumentNullException("La URL de la hoja de datos (DataSheetUrl) del componente no puede estar vacio",
                          nameof(value));
            _dataSheetUrl = value;
        }
    }

    public string Tipo
    {
        get => _tipo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El tipo de componente no puede estar vacio",
                          nameof(value));
            _tipo = value.Trim().ToLowerInvariant();
        }
    }

    public double PrecioReferencia
    {
        get => _precioReferencia;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El precio de referencia del componente debe ser un numero positivo: '{value}'",
                          nameof(value));
            _precioReferencia = value;
        }
    }

    public int EquipoId
    {
        get => _equipoId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del equipo debe ser un numero natural: '{value}'",
                          nameof(value));
            _equipoId = value;
        }
    }

    public Equipo Equipo
    {
        get => _equipo;
        private set => _equipo = value;
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Componente(int id, string nombre, string descripcion, string modelo,
                      Uri dataSheetUrl, string tipo, double precioReferencia,
                      int equipoId)
    {
        Id               = id;
        Nombre           = nombre;
        Descripcion      = descripcion;
        Modelo           = modelo;
        DataSheetUrl     = dataSheetUrl;
        Tipo             = tipo;
        PrecioReferencia = precioReferencia;
        EquipoId         = equipoId;
        EstaEliminado    = false;
    }

    public void Eliminar() => EstaEliminado = true;
}