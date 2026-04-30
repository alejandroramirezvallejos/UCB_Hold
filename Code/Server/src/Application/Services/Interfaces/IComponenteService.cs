using Ardalis.Result;

public interface IComponenteService
{
    Result<ComponenteDto> Crear(CrearComponenteComando comando);
    Result<List<ComponenteDto>> ObtenerTodos();
    Result<ComponenteDto> Actualizar(ActualizarComponenteComando comando);
    Result<ComponenteDto> Eliminar(EliminarComponenteComando comando);
}
