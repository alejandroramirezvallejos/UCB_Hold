public class ErrorEmpresaMantenimientoNoEncontrada : DomainException
{
    public ErrorEmpresaMantenimientoNoEncontrada() 
        : base("La empresa de mantenimiento especificada por nombre no existe o no está activa")
    {
    }
}