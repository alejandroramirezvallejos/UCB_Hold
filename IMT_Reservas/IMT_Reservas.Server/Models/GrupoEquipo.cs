public class GrupoEquipo
{
    private int    _id;
    private string _nombre;
    private string _modelo;
    private Uri    _dataSheetUrl;
    private int    _cantidad;
    private string _marca;
    private int    _categoriaId;
    private bool   _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del grupo de equipo debe ser un numero natural: '{value}'",
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
                throw new ArgumentException("El nombre del grupo de equipo no puede estar vacio",
                          nameof(value));
            _nombre = value.Trim();
        }
    }

    public string Modelo
    {
        get => _modelo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El modelo del grupo de equipo no puede estar vacio",
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
                throw new ArgumentException("La URL de la hoja de datos (DataSheetUrl) del grupo de equipo no puede estar vacia",
                          nameof(value));
            _dataSheetUrl = value;
        }
    }

    public int Cantidad
    {
        get => _cantidad;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"La cantidad de equipos debe ser un numero natural: '{value}'",
                          nameof(value));
            _cantidad = value;
        }
    }

    public string Marca
    {
        get => _marca;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La marca del grupo de equipo no puede estar vacia",
                          nameof(value));
            _marca = value.Trim();
        }
    }

    public int CategoriaId
    {
        get => _categoriaId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de categoria debe ser un numero natural: '{value}'",
                          nameof(value));
            _categoriaId = value;
        }
    }

    public Categoria Categoria { get; private set; }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public GrupoEquipo(int id, string nombre, string modelo, Uri dataSheetUrl, int cantidad,
                       string marca, int categoriaId)
    {
        Id            = id;
        Nombre        = nombre;
        Modelo        = modelo;
        DataSheetUrl  = dataSheetUrl;
        Cantidad      = cantidad;
        Marca         = marca;
        CategoriaId   = categoriaId;
        EstaEliminado = false;
    }
}
