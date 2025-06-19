public class ErrorEstadoPrestamoRequerido : DomainException
{
    public ErrorEstadoPrestamoRequerido() : base("El estado del préstamo es requerido.")
    {
    }
}