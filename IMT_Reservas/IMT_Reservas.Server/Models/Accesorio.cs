using System.ComponentModel;

public enum AccesorioTipo
{
    [Description("Estandar")]
    Estandar
}

public class Accesorio
{
    private int    _id;
    private string _nombre;
    private string _descripcion;
    private string _modelo;
    private Uri    _dataSheetUrl;
    private double _precio;
    private int    _equipoId;
    private string _tipo;
    private bool   _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del accesorio debe ser un numero natural: '{value}'", 
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
                throw new ArgumentException("El nombre del accesorio no puede estar vacio",
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
                throw new ArgumentException("La descripcion del accesorio no puede estar vacia", 
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
                throw new ArgumentException("El modelo del accesorio no puede estar vacio",
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
                throw new ArgumentNullException("La URL de la hoja de datos (DataSheetUrl) del accesorio no puede estar vacia",
                          nameof(value));
            _dataSheetUrl = value;
        }
    }

    public double Precio
    {
        get => _precio;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El precio del accesorio debe ser un numero positivo: '{value}'",
                          nameof(value));
            _precio = value;
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

    public Equipo Equipo { get; private set; }

    public string Tipo
    {
        get => _tipo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El tipo de accesorio de puede estar vacio",
                          nameof(value));

            var limpio = value.Trim().ToLowerInvariant();
            var nombres = Enum.GetNames(typeof(AccesorioTipo));
            foreach (var nombre in nombres)
            {
                if (nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase))
                {
                    _tipo = limpio;
                    return;
                }
            }
            throw new ArgumentException($"El tipo de accesorio ingresado es invalido: '{value}'",
                      nameof(value));
        }
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Accesorio(int id, string nombre, string descripcion, string modelo,
                     Uri dataSheetUrl, double precio, int equipoId, string tipo)
    {
        Id            = id;
        Nombre        = nombre;
        Descripcion   = descripcion;
        Modelo        = modelo;
        DataSheetUrl  = dataSheetUrl;
        Precio        = precio;
        EquipoId      = equipoId;
        Tipo          = tipo;
        EstaEliminado = false;
    }

    public void Eliminar() => EstaEliminado = true;
}
