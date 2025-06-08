public class Equipo : IEquipo, IEliminacionLogica
{
    private int      _id;
    private int      _grupoEquipoId;
    private string   _codigoImt            = string.Empty;
    private string?  _codigoUcb            = null;
    private string?  _descripcion          = null;
    private string   _estadoEquipo         = string.Empty;
    private string?  _numeroSerial         = null;
    private string?  _ubicacion            = null;
    private double?  _costoReferencia      = null;    
    private int?     _tiempoMaximoPrestamo = null;
    private string?  _procedencia          = null;
    private string?  _nombreGavetero       = null;
    private bool     _estaEliminado        = false;
    private DateOnly _fechaDeIngreso; 
    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del equipo");
    }

    public int GrupoEquipoId
    {
        get => _grupoEquipoId;
        private set => _grupoEquipoId = Verificar.SiEsNatural(value, "El ID del grupo de equipo");
    }

    public string CodigoImt
    {
        get => _codigoImt;
        private set => _codigoImt = Verificar.SiEsVacio(value, "El codigo IMT");
    }

    public string? CodigoUcb
    {
        get => _codigoUcb;
        private set => _codigoUcb = value is not null
                       ? Verificar.SiEsVacio(value, "El codigo UCB")
                       : null;
    }
    public string? Descripcion
    {
        get => _descripcion;
        private set => _descripcion = value is not null
                       ? Verificar.SiEsVacio(value, "La descripcion del equipo")
                       : null;
    }

    public string EstadoEquipo
    {
        get => _estadoEquipo;
        private set
        {
            Enum enumEstadoDelEquipo = Verificar.SiEstaEnEnum<EstadoDelEquipo>(value, "El estado del equipo");
            _estadoEquipo = enumEstadoDelEquipo.ToString();
        }
    }

    public string? NumeroSerial
    {
        get => _numeroSerial;
        private set => _numeroSerial = value is not null
                       ? Verificar.SiEsVacio(value, "El numero serial del equipo")
                       : null;
    }

    public string? Ubicacion
    {
        get => _ubicacion;
        private set => _ubicacion = value is not null
                       ? Verificar.SiEsVacio(value, "La ubicacion del equipo")
                       : null;
    }

    public double? CostoReferencia
    {
        get => _costoReferencia;
        private set => _costoReferencia = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El costo de referencia del equipo")
                       : null;
    }

    public int? TiempoMaximoPrestamo
    {
        get => _tiempoMaximoPrestamo;
        private set => _tiempoMaximoPrestamo = value.HasValue
                       ? Verificar.SiEsNatural(value.Value, "El tiempo maximo de prestamo del equipo")
                       : null;
    }

    public string? Procedencia
    {
        get => _procedencia;
        private set => _procedencia = value is not null
                       ? Verificar.SiEsVacio(value, "La procedencia del equipo")
                       : null;
    }    public string? NombreGavetero
    {
        get => _nombreGavetero;
        private set => _nombreGavetero = value is not null
                       ? Verificar.SiEsVacio(value, "El nombre del gavetero")
                       : null;
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public DateOnly FechaDeIngreso
    {
        get => _fechaDeIngreso;
        private set => _fechaDeIngreso = Verificar.SiNoEsFutura(value, "La fecha de ingreso del equipo");
    }    public Equipo(int grupoEquipoId, string codigoImt, string? codigoUcb, string? descripcion, 
                  string estadoEquipo, string? numeroSerial, string? ubicacion, 
                  double? costoReferencia, int? tiempoMaximoPrestamo, string? procedencia, 
                  string? nombreGavetero, DateOnly fechaDeIngreso)
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
        NombreGavetero       = nombreGavetero;
        FechaDeIngreso       = fechaDeIngreso;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
