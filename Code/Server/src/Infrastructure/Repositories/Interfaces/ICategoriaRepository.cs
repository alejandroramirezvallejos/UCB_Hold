using System.Data;
public interface ICategoriaRepository
{
    void Crear(CrearCategoriaComando comando);
    DataTable ObtenerTodos();
    void Actualizar(ActualizarCategoriaComando comando);
    void Eliminar(int id);
}