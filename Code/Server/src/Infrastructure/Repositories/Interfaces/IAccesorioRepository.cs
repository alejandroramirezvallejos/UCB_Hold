using System.Data;
using Ardalis.Result;

public interface IAccesorioRepository
{
    Result<AccesorioDto?> Crear(CrearAccesorioComando comando);
    Result<AccesorioDto?> Crear(int idEquipo, CrearAccesorioComando comando);
    Result<AccesorioDto?> Actualizar(ActualizarAccesorioComando comando);
    Result<AccesorioDto?> Actualizar(int? idEquipo, ActualizarAccesorioComando comando);
    Result<AccesorioDto?> Eliminar(EliminarAccesorioComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    int? ObtenerEquipoIdPorCodigoImt(int codigoImt);
}
