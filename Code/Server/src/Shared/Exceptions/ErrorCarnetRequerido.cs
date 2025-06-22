public class ErrorCarnetRequerido : DomainException
{
    public ErrorCarnetRequerido() 
        : base("El carnet es requerido")
    {
    }
}