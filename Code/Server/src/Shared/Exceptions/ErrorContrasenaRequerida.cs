public class ErrorContrasenaRequerida : DomainException
{
    public ErrorContrasenaRequerida() 
        : base("La contraseña es requerida")
    {
    }
}