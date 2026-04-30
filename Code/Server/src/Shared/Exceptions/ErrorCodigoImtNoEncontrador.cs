public class ErrorCodigoImtNoEncontrado : DomainException
{
    public ErrorCodigoImtNoEncontrado()
        : base("El código IMT no se encuentra registrado en ningun equipo activo")
    {
    }
}