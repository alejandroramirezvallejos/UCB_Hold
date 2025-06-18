public class ErrorNombreMuebleRequerido : DomainException
{
    public ErrorNombreMuebleRequerido() : base("El nombre del mueble es requerido")
    {
    }
}