public class ErrorTelefonoRequerido : DomainException
{
    public ErrorTelefonoRequerido() 
        : base("El teléfono es requerido")
    {
    }
}