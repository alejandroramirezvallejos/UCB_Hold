using System.Data;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;

public interface IPrestamoRepository
{
    Result<PrestamoDto> Eliminar(EliminarPrestamoComando comando);
    Result<DataTable> ObtenerTodos();
    bool ExisteActivoPorId(int id);
    bool ExisteGrupoEquipoActivoPorId(int id);
    bool ExisteUsuarioActivoPorCarnet(string carnet);
    Result<int> CrearPrestamo(CrearPrestamoComando comando);
    void CrearDetallePrestamo(int idPrestamo, int idEquipo);
    int? ObtenerEquipoDisponiblePorGrupo(int idGrupoEquipo, DateTime fechaPrestamoEsperada, DateTime fechaDevolucionEsperada);
    DataTable ObtenerEquipoPorId(int idEquipo);
    void GuardarContrato(int idPrestamo, IFormFile contrato);
    DataTable ObtenerPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo);
    void ActualizarEstado(ActualizarEstadoPrestamoComando comando);
    void ActualizarIdContrato(int prestamoId, string idContrato);
    List<byte[]> ObtenerContratoPorPrestamo(ObtenerContratoPorPrestamoConsulta consulta);
}
