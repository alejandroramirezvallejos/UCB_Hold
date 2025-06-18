
public class ErrorComponenteYaExiste : DomainException
{
    public ErrorComponenteYaExiste() 
        : base($"Ya existe un componente con el mismo nombre y modelo")
    {
    }
}

