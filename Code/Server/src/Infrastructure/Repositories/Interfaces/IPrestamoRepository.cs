using System.Data;

public interface IPrestamoRepository
{
    int Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
    void ActualizarEstado(ActualizarEstadoPrestamoComando comando);
    DataTable ObtenerPorCarnetYEstadoPrestamo(string carnet, string estadoPrestamo);
    void ActualizarIdContrato(int prestamoId, string idContrato);
    List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta);
}