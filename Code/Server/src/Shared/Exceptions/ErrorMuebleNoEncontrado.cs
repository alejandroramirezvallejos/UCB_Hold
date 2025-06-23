public class ErrorMuebleNoEncontrado : DomainException
{
    public ErrorMuebleNoEncontrado() 
        : base("El mueble especificado por nombre no existe o no está activo")
    {
    }
}