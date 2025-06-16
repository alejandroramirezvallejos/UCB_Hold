using System.Data;

public interface IAccesorioRepository
{
    void Crear(CrearAccesorioComando comando);
    DataTable ObtenerTodos();
    void Actualizar(ActualizarAccesorioComando comando);
    void Eliminar(int id);
}