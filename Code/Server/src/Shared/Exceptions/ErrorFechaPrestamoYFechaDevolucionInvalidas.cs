public class ErrorFechaPrestamoYFechaDevolucionInvalidas : DomainException
{
    public ErrorFechaPrestamoYFechaDevolucionInvalidas() 
        : base("La fecha de préstamo es posterior a la fecha de devolución")
    {
    }
}