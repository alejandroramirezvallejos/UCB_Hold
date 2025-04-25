using System.ComponentModel;

public enum EstadoDisponibilidad
{
    [Description("Disponible")]
    Disponible,
    [Description("Reservado")]
    Reservado,
    [Description("Revision")]
    Revision,
    [Description("Mantenimiento")]
    Mantenimiento
}

public enum EstadoEquipo
{
    [Description("Inoperativo")]
    Inoperativo,
    [Description("Parcialmente_Operativo")]
    Parcialmente_Operativo,
    [Description("Operativo")]
    Operativo
}

public class Equipo
{
    private int     _id;
    private int     _grupoEquipoId;
    private string  _codigoImt;
    private string  _codigoUcb;
    private string  _descripcion;
    private string  _estadoEquipo;
    private string  _numeroSerial;
    private string  _ubicacion;
    private double? _costoReferencia;
    private int?    _tiempoMaximoPrestamo;
    private string  _procedencia;
    private int     _gaveteroId;
    private string  _estadoDisponibilidad;
    private bool    _estaEliminado;
     
    public int Id
    {
        get => _id;
        private set => _id = value;
    }

    public int GrupoEquipoId
    {
        get => _grupoEquipoId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del grupo de equipo debe ser un numero natural : '{value}'",
                          nameof(value));
            _grupoEquipoId = value;
        }
    }

    public GrupoEquipo GrupoEquipo { get; private set; }

    public string CodigoImt
    {
        get => _codigoImt;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El codigo IMT no puede estar vacio",
                          nameof(value));
            _codigoImt = value.Trim();
        }
    }

    public string CodigoUcb
    {
        get => _codigoUcb;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El codigo UCB no puede estar vacio",
                          nameof(value));
            _codigoUcb = value.Trim();
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La descripcion del equipo no puede estar vacia",
                          nameof(value));
            _descripcion = value.Trim();
        }
    }

    public string EstadoEquipo
    {
        get => _estadoEquipo;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El estado del equipo no puede estar vacio",
                          nameof(value));

            var limpio = value.Trim();
            var nombres = Enum.GetNames(typeof(EstadoEquipo));

            foreach (var nombre in nombres)
            {
                if (nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase))
                {
                    _estadoEquipo = limpio;
                    return;
                }
            }

            throw new ArgumentException($"El estado de equipo es invalido: '{value}'",
                      nameof(value));
        }
    }

    public string NumeroSerial
    {
        get => _numeroSerial;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El numero de serie del equipo no puede estar vacio",
                          nameof(value));
            _numeroSerial = value.Trim();
        }
    }

    public string Ubicacion
    {
        get => _ubicacion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La ubicacion del equipo no puede estar vacia",
                          nameof(value));
            _ubicacion = value.Trim();
        }
    }

    public double? CostoReferencia
    {
        get => _costoReferencia;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El costo de referencia del equipo debe ser un numero positivo: '{value}'",
                          nameof(value));
            _costoReferencia = value;
        }
    }

    public int? TiempoMaximoPrestamo
    {
        get => _tiempoMaximoPrestamo;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El tiempo maximo de prestamo del equipo debe ser un numero natural: '{value}'", 
                          nameof(value));
            _tiempoMaximoPrestamo = value;
        }
    }

    public string Procedencia
    {
        get => _procedencia;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La procedencia del equipo no puede estar vacia",
                          nameof(value));
            _procedencia = value.Trim();
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

    public string EstadoDisponibilidad
    {
        get => _estadoDisponibilidad;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El estado de disponibilidad del equipo no puede estar vacio",
                          nameof(value));

            var limpio = value.Trim();
            var nombres = Enum.GetNames(typeof(EstadoDisponibilidad));

            foreach (var nombre in nombres)
            {
                if (nombre.Equals(limpio, StringComparison.OrdinalIgnoreCase))
                {
                    _estadoDisponibilidad = limpio;
                    return;
                }
            }

            throw new ArgumentException($"El estado de disponibilidad del equipo es invalido: '{value}'",
                      nameof(value));
        }
    }

    public Equipo(int grupoEquipoId, string codigoImt, string codigoUcb, string descripcion, 
                  string estadoEquipo, string numeroSerial, string ubicacion, 
                  double? costoReferencia, int? tiempoMaximoPrestamo, string procedencia, 
                  int gaveteroId, string estadoDisponibilidad)
    {
        GrupoEquipoId        = grupoEquipoId;
        CodigoImt            = codigoImt;
        CodigoUcb            = codigoUcb;
        Descripcion          = descripcion;
        EstadoEquipo         = estadoEquipo;
        NumeroSerial         = numeroSerial;
        Ubicacion            = ubicacion;
        CostoReferencia      = costoReferencia;
        TiempoMaximoPrestamo = tiempoMaximoPrestamo;
        Procedencia          = procedencia;
        GaveteroId           = gaveteroId;
        EstadoDisponibilidad = estadoDisponibilidad;
        EstaEliminado        = false;
    }
}
