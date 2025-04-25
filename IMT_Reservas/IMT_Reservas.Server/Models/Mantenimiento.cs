using System.ComponentModel;

public enum TipoMantenimiento
{
    [Description("Correctivo")]
    Correctivo,
    [Description("Preventivo")]
    Preventivo
}

public class Mantenimiento
{
    private int               _id;
    private TipoMantenimiento _tipoMantenimiento;
    private string            _descripcion;
    private string            _celularResponsable;
    private string            _detalle;
    private double           _costo;
    private DateOnly          _fechaMantenimiento;
    private int               _equipoId;
    private int               _empresaMantenimientoId;
    private bool              _estaEliminado;

    public int Id
    {
        get => _id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID del mantenimiento debe ser un numero natural: '{value}'",
                          nameof(value));
            _id = value;
        }
    }

    public TipoMantenimiento TipoMantenimiento
    {
        get => _tipoMantenimiento;
        private set
        {
            if (!Enum.IsDefined(typeof(TipoMantenimiento), value))
                throw new ArgumentException($"El tipo de mantenimiento es invalido: '{value}'",
                          nameof(value));
            _tipoMantenimiento = value;
        }
    }

    public string Descripcion
    {
        get => _descripcion;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La descripcion del mantenimiento no puede estar vacia",
                          nameof(value));
            _descripcion = value.Trim();
        }
    }

    public string CelularResponsable
    {
        get => _celularResponsable;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El celular del responsable no puede estar vacio",
                          nameof(value));
            _celularResponsable = value.Trim();
        }
    }

    public string Detalle
    {
        get => _detalle;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El detalle del mantenimiento no puede estar vacio",
                          nameof(value));
            _detalle = value.Trim();
        }
    }

    public double Costo
    {
        get => _costo;
        private set
        {
            if (value < 0)
                throw new ArgumentException($"El costo de mantenimiento debe ser un numero natural: '{value}'",
                          nameof(value));
            _costo = value;
        }
    }

    public DateOnly FechaMantenimiento
    {
        get => _fechaMantenimiento;
        private set
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (value > hoy)
                throw new ArgumentException($"La fecha de mantenimiento no puede ser futura: '{value}'",
                          nameof(value));
            _fechaMantenimiento = value;
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

    public int EmpresaMantenimientoId
    {
        get => _empresaMantenimientoId;
        private set
        {
            if (value <= 0)
                throw new ArgumentException($"El ID de la empresa de mantenimiento debe ser un numero natural: '{value}'",
                          nameof(value));
            _empresaMantenimientoId = value;
        }
    }

    public EmpresaMantenimiento EmpresaMantenimiento { get; private set; }

    public bool EstaEliminado
    {
        get => _estaEliminado;
        private set => _estaEliminado = value;
    }

    public Mantenimiento(TipoMantenimiento tipoMantenimiento, string descripcion, string celularResponsable,
                         string detalle, double costo, DateOnly fechaMantenimiento, int equipoId,
                         int empresaMantenimientoId)
    {
        TipoMantenimiento      = tipoMantenimiento;
        Descripcion            = descripcion;
        CelularResponsable     = celularResponsable;
        Detalle                = detalle;
        Costo                  = costo;
        FechaMantenimiento     = fechaMantenimiento;
        EquipoId               = equipoId;
        EmpresaMantenimientoId = empresaMantenimientoId;
        EstaEliminado          = false;
    }
}
