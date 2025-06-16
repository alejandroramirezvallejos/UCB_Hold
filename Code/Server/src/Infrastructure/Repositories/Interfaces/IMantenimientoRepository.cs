using System.Data;
public interface IMantenimientoRepository
{
    void Crear(CrearMantenimientoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
}