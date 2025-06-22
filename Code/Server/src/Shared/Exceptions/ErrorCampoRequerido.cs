public class ErrorCampoRequerido : DomainException
{
    public ErrorCampoRequerido(string campo) 
        : base($"El campo {campo} es requerido")
    {
    }
}

