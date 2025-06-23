public class ErrorGrupoEquipoNoEncontrado : DomainException
{
    public ErrorGrupoEquipoNoEncontrado() 
        : base("El grupo equipo especificado por nombre, marca y modelo no existe o no está activo")
    {
    }
}