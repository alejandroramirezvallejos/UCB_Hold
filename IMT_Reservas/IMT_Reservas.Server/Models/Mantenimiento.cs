public class Mantenimiento
{
    private int      _id;
    private string   _tipoMantenimiento;
    private string?  _descripcion = null;
    private double?  _costo = null;
    private DateOnly _fechaMantenimiento;
    private int      _empresaMantenimientoId;
    private bool     _estaEliminado = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del mantenimiento");
    }

    public string TipoMantenimiento
    {
        get => _tipoMantenimiento;
        private set
        {
            Enum enumTipoDeMantenimiento = Verificar.SiEstaEnEnum<TipoDeMantenimiento>(value, "El tipo de mantenimiento");
            _tipoMantenimiento = enumTipoDeMantenimiento.ToString();
        }
    }

    public string? Descripcion
    {
        get => _descripcion;
        private set => _descripcion = value is not null
                       ? Verificar.SiEsVacio(value, "La descripcion del mantenimiento")
                       : null;
    }

    public double? Costo
    {
        get => _costo;
        private set => _costo = value.HasValue
                       ? Verificar.SiEsPositivo(value.Value, "El costo del mantenimiento")
                       : null;
    }

    public DateOnly FechaMantenimiento
    {
        get => _fechaMantenimiento;
        private set => _fechaMantenimiento = Verificar.SiNoEsFutura(value, "La fecha de mantenimiento");
    }

    public int EmpresaMantenimientoId
    {
        get => _empresaMantenimientoId;
        private set => _empresaMantenimientoId = Verificar.SiEsNatural(value, "El ID de la empresa de mantenimiento");
    }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Mantenimiento(string tipoMantenimiento, string? descripcion,
                         double? costo, DateOnly fechaMantenimiento,
                         int empresaMantenimientoId)
    {
        TipoMantenimiento      = tipoMantenimiento;
        Descripcion            = descripcion;
        Costo                  = costo;
        FechaMantenimiento     = fechaMantenimiento;
        EmpresaMantenimientoId = empresaMantenimientoId;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
