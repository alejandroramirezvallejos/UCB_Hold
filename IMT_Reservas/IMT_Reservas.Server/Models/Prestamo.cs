public class Prestamo
{
    private int      _id;
    private DateTime _fechaSolicitud;
    private DateTime _fechaPrestamo;
    private DateTime _fechaDevolucion;
    private DateTime _fechaDevolucionEsperada;
    private string?  _observacion = null;
    private string   _estadoPrestamo;
    private string   _carnetUsuario;
    private int      _equipoId;
    private bool     _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del prestamo");
    }

    public DateTime FechaSolicitud
    {
        get => _fechaSolicitud;
        private set => _fechaSolicitud = Verificar.SiNoEsFutura(value, "La fecha de solicitud del prestamo");
    }

    public DateTime FechaPrestamo
    {
        get => _fechaPrestamo;
        private set => _fechaPrestamo = Verificar.SiNoEsAnteriorA(value, FechaSolicitud, "La fecha del prestamo", "la fecha de solicitud del prestamo");
    }

    public DateTime FechaDevolucion
    {
        get => _fechaDevolucion;
        private set => _fechaDevolucion = Verificar.SiNoEsAnteriorA(value, FechaPrestamo, "La fecha de devolucion del prestamo", "la fecha del prestamo");
    }

    public DateTime FechaDevolucionEsperada
    {
        get => _fechaDevolucionEsperada;
        private set => _fechaDevolucionEsperada = Verificar.SiNoEsAnteriorA(value, FechaPrestamo, "La fecha de devolucion esperada", "la fecha del prestamo");
    }

    public string? Observacion
    {
        get => _observacion;
        private set => _observacion = value is not null
                       ? Verificar.SiEsVacio(value, "La observacion del prestamo")
                       : null;
    }

    public string EstadoPrestamo
    {
        get => _estadoPrestamo;
        private set
        {
            Enum enumEstadoPrestamo = Verificar.SiEstaEnEnum<EstadoDelPrestamo>(value, "El estado del prestamo");
            _estadoPrestamo = enumEstadoPrestamo.ToString();
        }
    }

    public string CarnetUsuario
    {
        get => _carnetUsuario;
        private set => _carnetUsuario = Verificar.SiEsVacio(value, "El carnet de usuario");
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

    public Prestamo(DateTime fechaSolicitud, DateTime fechaPrestamo,
                    DateTime fechaDevolucion, DateTime fechaDevolucionEsperada, 
                    string? observacion, string estado,
                    string carnetUsuario, int equipoId)
    {
        FechaSolicitud          = fechaSolicitud;
        FechaPrestamo           = fechaPrestamo;
        FechaDevolucion         = fechaDevolucion;
        FechaDevolucionEsperada = fechaDevolucionEsperada;
        Observacion             = observacion;
        EstadoPrestamo          = estado;
        CarnetUsuario           = carnetUsuario;
        EquipoId                = equipoId;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
