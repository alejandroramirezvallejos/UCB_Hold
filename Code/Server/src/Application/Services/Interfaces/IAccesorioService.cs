using Ardalis.Result;

public interface IAccesorioService
{
    Result<AccesorioDto> Crear(CrearAccesorioComando comando);
    Result<List<AccesorioDto>> ObtenerTodos();
    Result<AccesorioDto> Actualizar(ActualizarAccesorioComando comando);
    Result<AccesorioDto> Eliminar(EliminarAccesorioComando comando);
}
