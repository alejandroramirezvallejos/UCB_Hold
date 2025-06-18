public class ErrorFechaMantenimientoFinalRequerida : DomainException
{
    public ErrorFechaMantenimientoFinalRequerida() : base("La fecha de finalización del mantenimiento es requerida")
    {
    }
}
