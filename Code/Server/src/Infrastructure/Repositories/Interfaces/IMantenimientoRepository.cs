using System.Data;
using Ardalis.Result;

public interface IMantenimientoRepository
{
    Result<MantenimientoDto> Crear(CrearMantenimientoComando comando);
    Result<MantenimientoDto> Eliminar(EliminarMantenimientoComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    int? ObtenerEmpresaIdPorNombre(string nombreEmpresa);
    int? ObtenerEquipoIdPorCodigoImt(int codigoImt);
    Result<int> CrearMantenimiento(int idEmpresa, CrearMantenimientoComando comando);
    void CrearDetalleMantenimiento(int idMantenimiento, int idEquipo, string? tipoMantenimiento, string? descripcionEquipo);
}
