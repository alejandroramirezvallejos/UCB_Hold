using System.Data;
using Ardalis.Result;

public interface ICarreraRepository
{
    Result<CarreraDto?> Crear(CrearCarreraComando comando);
    Result<CarreraDto?> Actualizar(ActualizarCarreraComando comando);
    Result<CarreraDto?> Eliminar(EliminarCarreraComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivaPorId(int id);
    bool ExisteActivaPorNombre(string nombre);
    bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir);
    bool ReactivarEliminadaPorNombre(string nombre);
    void EliminarLogicamentePorId(int id);
}
