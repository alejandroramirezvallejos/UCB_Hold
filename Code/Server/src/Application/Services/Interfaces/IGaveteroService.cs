using Ardalis.Result;

public interface IGaveteroService
{
    Result<GaveteroDto> Crear(CrearGaveteroComando comando);
    Result<List<GaveteroDto>> ObtenerTodos();
    Result<GaveteroDto> Actualizar(ActualizarGaveteroComando comando);
    Result<GaveteroDto> Eliminar(EliminarGaveteroComando comando);
}
