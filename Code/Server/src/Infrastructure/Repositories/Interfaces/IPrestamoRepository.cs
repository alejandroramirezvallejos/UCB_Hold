using System.Data;
public interface IPrestamoRepository
{
    void Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
    void ActualizarEstado(ActualizarEstadoPrestamoComando comando);
    DataTable ObtenerPorCarnetYEstadoPrestamo(string carnet, string estadoPrestamo);
}