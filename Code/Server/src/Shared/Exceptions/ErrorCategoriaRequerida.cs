public class ErrorCategoriaRequerida : DomainException
{
    public ErrorCategoriaRequerida() : base("La categoría es requerida")
    {
    }
}