
public class ErrorRegistroEnUso : DomainException
{
    public ErrorRegistroEnUso()
        : base($"No se puede eliminar el registro porque está siendo utilizado")
    {
    }
}
