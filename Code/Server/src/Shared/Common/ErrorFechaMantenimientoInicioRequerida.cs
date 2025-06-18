public class ErrorFechaMantenimientoInicioRequerida : DomainException
{
    public ErrorFechaMantenimientoInicioRequerida() : base("La fecha de inicio del mantenimiento es requerida")
    {
    }
}