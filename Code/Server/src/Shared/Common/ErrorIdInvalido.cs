
public class ErrorIdInvalido : DomainException
{
    public ErrorIdInvalido() 
        : base("El ID es requerido y debe ser mayor a 0")
    {
    }
}

