public class ErrorCodigoImtYTiposLongitudDiferente : DomainException
{
    public ErrorCodigoImtYTiposLongitudDiferente() : base("Los arreglos de códigos IMT y tipos de mantenimiento deben tener la misma longitud")
    {
    }
}
