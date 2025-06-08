public class DetalleMantenimiento : IDetalleMantenimiento, IEliminacionLogica
{
    private int     _id;
    private int     _idMantenimiento; 
    private string? _descripcion      = null;
    private int     _idEquipo;
    private bool    _estaEliminado    = false; 

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del detalle de mantenimiento");
    }

    public int IdMantenimiento
    {
        get => _idMantenimiento;
        private set => _idMantenimiento = Verificar.SiEsNatural(value, "El ID del mantenimiento");
    }

    public string? Descripcion
    {
        get => _descripcion;
        private set => _descripcion = value is not null
                       ? Verificar.SiEsVacio(value, "La descripcion del detalle de mantenimiento")
                       : null;
    }

    public int IdEquipo
    {
        get => _idEquipo;
        private set => _idEquipo = Verificar.SiEsNatural(value, "El ID del equipo");
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }
    
    public DetalleMantenimiento(int idMantenimiento, string? descripcion, int idEquipo)
    {
        IdMantenimiento = idMantenimiento;
        Descripcion     = descripcion;
        IdEquipo        = idEquipo;
    }
    
    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
