using System.Data;
using Ardalis.Result;

public interface IGaveteroRepository
{
    Result<GaveteroDto?> Crear(CrearGaveteroComando comando);
    Result<GaveteroDto?> Crear(int idMueble, CrearGaveteroComando comando);
    Result<GaveteroDto?> Actualizar(ActualizarGaveteroComando comando);
    Result<GaveteroDto?> Actualizar(int? idMueble, ActualizarGaveteroComando comando);
    Result<GaveteroDto?> Eliminar(EliminarGaveteroComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    bool ExisteActivoPorNombre(string nombre);
    bool ExisteActivoPorNombreExcluyendoId(string nombre, int idExcluir);
    int? ObtenerMuebleIdPorNombre(string nombreMueble);
    int? ObtenerMuebleIdPorGaveteroId(int gaveteroId);
}
