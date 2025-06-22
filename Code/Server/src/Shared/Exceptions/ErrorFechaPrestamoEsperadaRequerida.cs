public class ErrorFechaPrestamoEsperadaRequerida : DomainException
{
    public ErrorFechaPrestamoEsperadaRequerida() : base("La fecha de préstamo esperada es requerida")
    {
    }
}