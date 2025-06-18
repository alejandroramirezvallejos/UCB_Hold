public class ErrorDescripcionRequerida: DomainException
{
    public ErrorDescripcionRequerida() : base("La descripción es requerida")
    {
    }
}