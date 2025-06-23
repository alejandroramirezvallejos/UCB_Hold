public class ErrorGaveteroNoEncontrado : DomainException
{
    public ErrorGaveteroNoEncontrado() 
        : base("El gavetero especificado por nombre no existe o no está activo")
    {
    }
}