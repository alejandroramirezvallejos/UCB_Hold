public class ErrorApellidoMaternoRequerido : DomainException
{
    public ErrorApellidoMaternoRequerido() 
        : base("El apellido materno es requerido")
    {
    }
}