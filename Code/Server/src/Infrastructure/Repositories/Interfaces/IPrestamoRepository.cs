using System.Data;
public interface IPrestamoRepository
{
    int Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
    void AceptarPrestamo(int prestamoId);
    void ActualizarIdContrato(int prestamoId, string idContrato);
    void ActualizarEstado(ActualizarEstadoPrestamoComando comando);
}