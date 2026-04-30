using System.Data;
using Ardalis.Result;

public interface ICategoriaRepository
{
    Result<CategoriaDto> Crear(CrearCategoriaComando comando);
    Result<CategoriaDto> Actualizar(ActualizarCategoriaComando comando);
    Result<CategoriaDto> Eliminar(EliminarCategoriaComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivaPorId(int id);
    bool ExisteActivaPorNombre(string nombre);
    bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir);
    bool ReactivarEliminadaPorNombre(string nombre);
    void EliminarLogicamentePorId(int id);
}
