public class Mantenimiento : IMantenimiento, IEliminacionLogica
{
    private int      _id;
    private string   _tipo                       = string.Empty;
    private string?  _descripcion                = null;
    private double?  _costo                      = null;
    private DateOnly _fechaMantenimiento;
    private DateOnly _fechaFinalDeMantenimiento; 
    private int      _empresaMantenimientoId;
    private bool     _estaEliminado              = false;

    public int Id
    {
        get => _id;
        private set => _id = Verificar.SiEsNatural(value, "El ID del mantenimiento");
    }

    public string Tipo
    {
        get => _tipo;
        private set
        {
            Enum enumTipoDeMantenimiento = Verificar.SiEstaEnEnum<TipoDeMantenimiento>(value, "El tipo de mantenimiento");
            _tipo = enumTipoDeMantenimiento.ToString();
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

    public DateOnly FechaFinalDeMantenimiento
    {
        get => _fechaFinalDeMantenimiento;
        private set => _fechaFinalDeMantenimiento = Verificar.SiNoEsPosteriorA(value, FechaMantenimiento, "La fecha final de mantenimiento", "la fecha de mantenimiento");
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

    public Mantenimiento(string tipo, string? descripcion, double? costo,
                         DateOnly fechaMantenimiento, int empresaMantenimientoId,
                         DateOnly fechaFinalDeMantenimiento)
    {
        Tipo                      = tipo;
        Descripcion               = descripcion;
        Costo                     = costo;
        FechaMantenimiento        = fechaMantenimiento;
        EmpresaMantenimientoId    = empresaMantenimientoId;
        FechaFinalDeMantenimiento = fechaFinalDeMantenimiento;
    }

    public void Eliminar() => EstaEliminado = true;

    public void Recuperar() => EstaEliminado = false;
}
