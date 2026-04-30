using System.Data;
using Ardalis.Result;

public interface IMuebleRepository
{
    Result<MuebleDto?> Crear(CrearMuebleComando comando);
    Result<MuebleDto?> Actualizar(ActualizarMuebleComando comando);
    Result<MuebleDto?> Eliminar(EliminarMuebleComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    void ActualizarNumeroGaveteros(int idMueble, int incremento);
}
