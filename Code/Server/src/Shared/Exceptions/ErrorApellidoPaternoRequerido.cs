public class ErrorApellidoPaternoRequerido : DomainException
{
    public ErrorApellidoPaternoRequerido() 
        : base("El apellido paterno es requerido")
    {
    }
}