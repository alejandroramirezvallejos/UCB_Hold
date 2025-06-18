
public class ErrorRegistroNoEncontrado : DomainException
{
    public ErrorRegistroNoEncontrado() 
        : base($"No se encontró el registro especificado")
    {
    }
}

