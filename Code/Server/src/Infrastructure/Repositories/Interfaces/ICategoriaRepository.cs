public interface ICategoriaRepository
{
    void Crear(CrearCategoriaComando comando);
    List<CategoriaDto> ObtenerTodos();
    void Actualizar(ActualizarCategoriaComando comando);
    void Eliminar(int id);
}