public class ErrorEstadoPrestamoInvalido : DomainException
{
    public ErrorEstadoPrestamoInvalido() : base("El estado del préstamo es inválido.")
    {
    }
}