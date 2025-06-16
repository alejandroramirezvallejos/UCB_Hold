using System.Data;
public interface IEquipoRepository
{
    void Crear(CrearEquipoComando comando);
    void Actualizar(ActualizarEquipoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
}