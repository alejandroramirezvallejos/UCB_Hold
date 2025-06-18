
public class ErrorRegistroYaExiste : DomainException
{
    public ErrorRegistroYaExiste() 
        : base($"Ya existe un registro con estos datos")
    {
    }
}

