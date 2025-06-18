
public class ErrorValorNegativo : DomainException
{
    public ErrorValorNegativo(string campo) 
        : base($"El {campo} no puede ser negativo")
    {
    }
}
