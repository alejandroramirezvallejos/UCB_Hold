using System.Data;
using Ardalis.Result;

public interface IEmpresaMantenimientoRepository
{
    Result<EmpresaMantenimientoDto> Crear(CrearEmpresaMantenimientoComando comando);
    Result<EmpresaMantenimientoDto> Actualizar(ActualizarEmpresaMantenimientoComando comando);
    Result<EmpresaMantenimientoDto> Eliminar(EliminarEmpresaMantenimientoComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivaPorId(int id);
    bool ExisteActivaPorNombre(string nombre);
    bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir);
    bool ReactivarEliminadaPorNombre(string nombre);
    void EliminarLogicamentePorId(int id);
    int? ObtenerIdPorNombre(string nombre);
}
