public class ErrorFechaDevolucionEsperadaRequerida : DomainException
{
    public ErrorFechaDevolucionEsperadaRequerida() : base("La fecha de devolución esperada es requerida")
    {
    }
}