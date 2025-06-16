public interface ICategoriaService
{
    void CrearCategoria(CrearCategoriaComando comando);
    List<CategoriaDto>? ObtenerTodasCategorias();
    void ActualizarCategoria(ActualizarCategoriaComando comando);
    void EliminarCategoria(EliminarCategoriaComando comando);
}
