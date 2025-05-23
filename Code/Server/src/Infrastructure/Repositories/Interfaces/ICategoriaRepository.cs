public interface ICategoriaRepository
{
    CategoriaDto Crear(CrearCategoriaComando comando);
    CategoriaDto? ObtenerPorId(int id);
    List<CategoriaDto> ObtenerTodos();
    CategoriaDto? Actualizar(ActualizarCategoriaComando comando);
    bool Eliminar(int id);
}