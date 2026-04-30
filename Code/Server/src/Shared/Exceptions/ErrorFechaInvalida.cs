
public class ErrorFechaInvalida : DomainException
{
    public ErrorFechaInvalida()
        : base($"La fecha es inválida")
    {
    }
}

