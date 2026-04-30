using Ardalis.Result;

public interface IMantenimientoService
{
    Result<MantenimientoDto> Crear(CrearMantenimientoComando comando);
    Result<List<MantenimientoDto>> ObtenerTodos();
    Result<MantenimientoDto> Eliminar(EliminarMantenimientoComando comando);
}
