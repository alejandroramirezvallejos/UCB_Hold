using Ardalis.Result;

public interface ICarreraService
{
    Result<CarreraDto> Crear(CrearCarreraComando comando);
    Result<List<CarreraDto>> ObtenerTodos();
    Result<CarreraDto> Actualizar(ActualizarCarreraComando comando);
    Result<CarreraDto> Eliminar(EliminarCarreraComando comando);
}
