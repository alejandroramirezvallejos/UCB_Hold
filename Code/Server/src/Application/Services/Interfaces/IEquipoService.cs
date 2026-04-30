using Ardalis.Result;

public interface IEquipoService
{
    Result<EquipoDto> Crear(CrearEquipoComando comando);
    Result<List<EquipoDto>> ObtenerTodos();
    Result<EquipoDto> Actualizar(ActualizarEquipoComando comando);
    Result<EquipoDto> Eliminar(EliminarEquipoComando comando);
}
