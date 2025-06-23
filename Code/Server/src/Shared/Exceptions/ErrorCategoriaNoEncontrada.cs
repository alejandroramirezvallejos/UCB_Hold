public class ErrorCategoriaNoEncontrada : DomainException
{
    public ErrorCategoriaNoEncontrada() 
        : base("La categoría especificada por nombre no existe o no está activa")
    {
    }
}