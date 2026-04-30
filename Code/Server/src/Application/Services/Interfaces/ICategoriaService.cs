using Ardalis.Result;

public interface ICategoriaService
{
    Result<CategoriaDto> Crear(CrearCategoriaComando comando);
    Result<List<CategoriaDto>> ObtenerTodos();
    Result<CategoriaDto> Actualizar(ActualizarCategoriaComando comando);
    Result<CategoriaDto> Eliminar(EliminarCategoriaComando comando);
}
