using Ardalis.Result;

public interface IMuebleService
{
    Result<MuebleDto> Crear(CrearMuebleComando comando);
    Result<List<MuebleDto>> ObtenerTodos();
    Result<MuebleDto> Actualizar(ActualizarMuebleComando comando);
    Result<MuebleDto> Eliminar(EliminarMuebleComando comando);
}
