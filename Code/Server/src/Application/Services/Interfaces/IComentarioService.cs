using Ardalis.Result;

public interface IComentarioService
{
    Result<ComentarioDto> Crear(CrearComentarioComando comando);
    Result<ComentarioDto> Eliminar(EliminarComentarioComando comando);
    List<ComentarioDto>? ObtenerComentariosPorGrupoEquipo(ObtenerComentariosPorGrupoEquipoConsulta consulta);
    void AgregarLikeComentario(AgregarLikeComentarioComando comando);
    void QuitarLikeComentario(QuitarLikeComentarioComando comando);
}
