public class ErrorCarreraRequerida : DomainException
{
    public ErrorCarreraRequerida() 
        : base("La carrera es requerida")
    {
    }
}