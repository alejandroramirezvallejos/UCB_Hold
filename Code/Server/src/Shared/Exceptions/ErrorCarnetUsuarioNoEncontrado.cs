
public class ErrorCarnetUsuarioNoEncontrado : DomainException
{
    public ErrorCarnetUsuarioNoEncontrado()
        : base("No se encontró un usuario con el carnet especificado")
    {
    }
}
