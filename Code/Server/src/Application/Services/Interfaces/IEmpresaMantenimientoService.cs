using Ardalis.Result;

public interface IEmpresaMantenimientoService
{
    Result<EmpresaMantenimientoDto> Crear(CrearEmpresaMantenimientoComando comando);
    Result<List<EmpresaMantenimientoDto>> ObtenerTodos();
    Result<EmpresaMantenimientoDto> Actualizar(ActualizarEmpresaMantenimientoComando comando);
    Result<EmpresaMantenimientoDto> Eliminar(EliminarEmpresaMantenimientoComando comando);
}
