using System.Data;
public interface IPrestamoRepository
{
    void Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
}