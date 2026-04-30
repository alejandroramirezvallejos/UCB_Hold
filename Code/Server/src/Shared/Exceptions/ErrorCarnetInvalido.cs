
public class ErrorCarnetInvalido : DomainException
{
    public ErrorCarnetInvalido()
        : base($"El carnet no es válido")
    {
    }
}

