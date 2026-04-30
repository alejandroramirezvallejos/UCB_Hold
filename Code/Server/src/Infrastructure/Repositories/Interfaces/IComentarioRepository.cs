using System.Data;
using Ardalis.Result;

public interface IComentarioRepository
{
    Result<ComentarioDto> Crear(CrearComentarioComando comando);
    Result<ComentarioDto> Eliminar(EliminarComentarioComando comando);
    Result<ComentarioDto> AgregarLike(AgregarLikeComentarioComando comando);
    Result<ComentarioDto> QuitarLike(QuitarLikeComentarioComando comando);
    DataTable ObtenerPorGrupoEquipo(int idGrupoEquipo);
}
