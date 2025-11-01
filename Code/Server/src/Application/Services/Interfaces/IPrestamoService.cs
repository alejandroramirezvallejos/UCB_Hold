using IMT_Reservas.Server.Application.ResponseDTOs;

public interface IPrestamoService
{
    PrestamoConEquiposDto CrearPrestamo(CrearPrestamoComando comando);
    List<PrestamoDto>? ObtenerTodosPrestamos();
    List<PrestamoDto>? ObtenerPrestamosPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo);
    void EliminarPrestamo(EliminarPrestamoComando comando);
    void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando);
    List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta);
}
