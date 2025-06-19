namespace IMT_Reservas.Server.Application.Interfaces
{
    public interface IComentarioService
    {
        void CrearComentario(CrearComentarioComando comando);
        List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta);
        void EliminarComentario(EliminarComentarioComando comando);
        void AgregarLikeComentario(AgregarLikeComentarioComando comando);
    }
}