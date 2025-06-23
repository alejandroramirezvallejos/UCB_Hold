using System.Data;

public interface IComentarioRepository
{
    void Crear(CrearComentarioComando comando);
    void Eliminar(EliminarComentarioComando comando);
    void AgregarLike(AgregarLikeComentarioComando comando);
    void QuitarLike(QuitarLikeComentarioComando comando);
    DataTable ObtenerPorGrupoEquipo(int idGrupoEquipo);
}
