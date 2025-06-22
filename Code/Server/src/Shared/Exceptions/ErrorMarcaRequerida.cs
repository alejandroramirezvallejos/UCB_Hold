public class ErrorMarcaRequerida : DomainException
{
    public ErrorMarcaRequerida() : base("La marca es requerida")
    {
    }
}