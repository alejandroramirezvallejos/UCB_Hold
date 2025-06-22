public class ErrorRolInvalido : DomainException
{
    public ErrorRolInvalido() :
    base("El rol especificado no es válido, debe ser 'administrador' o 'estudiante'")
    {
    }
}