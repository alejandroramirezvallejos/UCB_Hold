public class ErrorContratoNoNulo : DomainException
{
    public ErrorContratoNoNulo() 
        : base("El contrato especificado no puede ser nulo")
    {
    }
}