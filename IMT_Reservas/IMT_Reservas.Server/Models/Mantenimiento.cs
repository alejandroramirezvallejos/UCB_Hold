public enum TipoMantenimiento { Correctivo, Preventivo } 

public class Mantenimiento
{
    public int                  Id                     { get; private set; }
    public TipoMantenimiento    TipoMantenimiento      { get; private set; }
    public string               Descripcion            { get; private set; }
    public string               CelularResponsable     { get; private set; }
    public string               Detalle                { get; private set; }
    public decimal              Costo                  { get; private set; }
    public DateOnly             FechaMantenimiento     { get; private set; }
    public int                  EquipoId               { get; private set; }
    public Equipo               Equipo                 { get; private set; }
    public int                  EmpresaMantenimientoId { get; private set; }
    public EmpresaMantenimiento EmpresaMantenimiento   { get; private set; }
    public bool                 EstaEliminado          { get; private set; }

    public Mantenimiento(TipoMantenimiento tipoMantenimiento, string descripcion, string celularResponsable, 
                         string detalle, decimal costo, DateOnly fechaMantenimiento, int equipoId, 
                         int empresaMantenimientoId)
    {
        TipoMantenimiento        = tipoMantenimiento;
        Descripcion              = descripcion;
        CelularResponsable       = celularResponsable;
        Detalle                  = detalle;
        Costo                    = costo;
        FechaMantenimiento       = fechaMantenimiento;
        EquipoId                 = equipoId;
        EmpresaMantenimientoId   = empresaMantenimientoId;
        EstaEliminado            = false;
    }
}
