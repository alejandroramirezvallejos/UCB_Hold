using System.Data;
using IMT_Reservas.Server.Application.ResponseDTOs;

public interface IPrestamoRepository
{
    PrestamoConEquiposDto Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    DataTable ObtenerTodos();
    void ActualizarEstado(ActualizarEstadoPrestamoComando comando);
    DataTable ObtenerPorCarnetYEstadoPrestamo(string carnet, string estadoPrestamo);
    void ActualizarIdContrato(int prestamoId, string idContrato);
    List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta);
}