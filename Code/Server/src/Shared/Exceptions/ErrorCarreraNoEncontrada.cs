public class ErrorCarreraNoEncontrada : DomainException
{
    public ErrorCarreraNoEncontrada() 
        : base("La carrera especificada por nombre no existe o no está activa")
    {
    }
}