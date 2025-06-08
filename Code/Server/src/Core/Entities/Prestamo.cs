public class Prestamo : IPrestamo, IEliminacionLogica
{
    private int       _id;
    private DateTime? _fechaPrestamo            = null;
    private DateTime? _fechaDevolucion          = null;
    private DateTime  _fechaDevolucionEsperada;
    private DateTime  _fechaPrestamoEsperado;
    private string?   _observacion              = null;
    private string    _estadoPrestamo           = string.Empty;
    private string    _carnetUsuario            = string.Empty;
    private int       _equipoId;
    private bool      _estaEliminado            = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del prestamo");
    }

    public DateTime? FechaPrestamo
    {
        get => _fechaPrestamo;
        private set
        {
            if (value.HasValue)
            {
                _fechaPrestamo = Verificar.SiNoEsAnteriorA(value.Value, FechaPrestamoEsperado, "La fecha del préstamo", "la fecha de préstamo esperada");
            }
            else
            {
                _fechaPrestamo = null;
            }
        }
    }

    public DateTime? FechaDevolucion 
    {
        get => _fechaDevolucion;
        private set 
        {
            if (value.HasValue)
            {
                if (!FechaPrestamo.HasValue)
                {
                    throw new InvalidOperationException("No se puede establecer FechaDevolucion si FechaPrestamo no está establecida.");
                }
                _fechaDevolucion = Verificar.SiNoEsAnteriorA(value.Value, FechaPrestamo.Value, "La fecha de devolucion del prestamo", "la fecha del prestamo");
            }
            else
            {
                _fechaDevolucion = null;
            }
        }
    }

    public DateTime FechaDevolucionEsperada
    {
        get => _fechaDevolucionEsperada;
        private set => _fechaDevolucionEsperada = Verificar.SiNoEsAnteriorA(value, FechaPrestamoEsperado, "La fecha de devolucion esperada", "la fecha de prestamo esperada");
    }

    public DateTime FechaPrestamoEsperado
    {
        get => _fechaPrestamoEsperado;
        private set => _fechaPrestamoEsperado = value; 
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

    public Prestamo(
        DateTime? fechaPrestamo,
        DateTime? fechaDevolucion,
        DateTime fechaDevolucionEsperada,
        DateTime fechaPrestamoEsperado,
        string? observacion, string estado,
        string carnetUsuario, int equipoId)
    {
        FechaPrestamoEsperado   = fechaPrestamoEsperado;
        FechaDevolucionEsperada = fechaDevolucionEsperada;
        FechaPrestamo           = fechaPrestamo;
        FechaDevolucion         = fechaDevolucion;
        Observacion             = observacion;
        EstadoPrestamo          = estado;
        CarnetUsuario           = carnetUsuario;
        EquipoId                = equipoId;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
