using Ardalis.Result;
using IMT_Reservas.Server.Application.ResponseDTOs;

public interface IPrestamoService
{
    Result<PrestamoConEquiposDto> Crear(CrearPrestamoComando comando);
    Result<List<PrestamoDto>> ObtenerTodos();
    Result<PrestamoDto> Eliminar(EliminarPrestamoComando comando);
    void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando);
    List<PrestamoDto>? ObtenerPrestamosPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo);
    List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta);
}
