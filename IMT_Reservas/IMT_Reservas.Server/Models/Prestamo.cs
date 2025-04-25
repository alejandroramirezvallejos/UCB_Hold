using System.ComponentModel;

public enum PrestamoEstado
{
    [Description("Pendiente")]
    Pendiente,
    [Description("Rechazado")]
    Rechazado,
    [Description("Aprobado")]
    Aprobado,
    [Description("Activo")]
    Activo,
    [Description("Finalizado")]
    Finalizado,
    [Description("Cancelado")]
    Cancelado
}

public class Prestamo
{
    private int            _id;
    private DateTimeOffset _fechaSolicitud;
    private DateTimeOffset _fechaPrestamo;
    private DateTimeOffset _fechaDevolucion;
    private string         _observacion;
    private PrestamoEstado _estadoPrestamo;
    private string         _carnetUsuario;
    private int            _equipoId;
    private bool           _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del prestamo debe ser un numero natural: '{value}'", 
                          nameof(value));
            _id = value;
        }
    }

    public DateTimeOffset FechaSolicitud
    {
        get => _fechaSolicitud;
        private set
        {
            var ahora = DateTimeOffset.Now;
            if (value > ahora)
                throw new ArgumentException($"La fecha de solicitud del prestamo no puede ser futura: '{value}'",
                          nameof(value));
            _fechaSolicitud = value;
        }
    }

    public DateTimeOffset FechaPrestamo
    {
        get => _fechaPrestamo;
        private set
        {
            if (value < _fechaSolicitud)
                throw new ArgumentException($"La fecha de prestamo no puede ser anterior a la solicitud: '{value}'",
                          nameof(value));
            _fechaPrestamo = value;
        }
    }

    public DateTimeOffset FechaDevolucion
    {
        get => _fechaDevolucion;
        private set
        {
            if (value < _fechaPrestamo)
                throw new ArgumentException($"La fecha de devolucion no puede ser anterior al prestamo: '{value}'",
                          nameof(value));
            _fechaDevolucion = value;
        }
    }

    public string Observacion
    {
        get => _observacion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La observacion no puede estar vacia",
                          nameof(value));
            _observacion = value.Trim();
        }
    }

    public PrestamoEstado EstadoPrestamo
    {
        get => _estadoPrestamo;
        private set
        {
            if (!Enum.IsDefined(typeof(PrestamoEstado), value))
                throw new ArgumentException($"El estado de prestamo es invalido: '{value}'",
                          nameof(value));
            _estadoPrestamo = value;
        }
    }

    public string CarnetUsuario
    {
        get => _carnetUsuario;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El carnet de usuario no puede estar vacio",
                          nameof(value));
            _carnetUsuario = value.Trim();
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

    public Usuario Usuario { get; private set; }
    public Equipo Equipo { get; private set; }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Prestamo(DateTimeOffset fechaSolicitud, DateTimeOffset fechaPrestamo,
                    DateTimeOffset fechaDevolucion, string observacion, PrestamoEstado estado,
                    string carnetUsuario, int equipoId)
    {
        FechaSolicitud  = fechaSolicitud;
        FechaPrestamo   = fechaPrestamo;
        FechaDevolucion = fechaDevolucion;
        Observacion     = observacion;
        EstadoPrestamo  = estado;
        CarnetUsuario   = carnetUsuario;
        EquipoId        = equipoId;
        EstaEliminado   = false;
    }
}
