using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Infrastructure.Repositories.Interfaces;
using IMT_Reservas.Server.Shared.Common;
using System.Data;
using System.Text.Json;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarritoRepository : ICarritoRepository
{
    private readonly IExecuteQuery _executeQuery;
    public CarritoRepository(IExecuteQuery executeQuery) => _executeQuery = executeQuery;

    public IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito)
    {
        var jsonCarrito = JsonSerializer.Serialize(carrito);
        var query = "SELECT * FROM obtener_fechas_no_disponibles_por_id_grupos_equipos(@fechaInicio, @fechaFin, @jsonInput::jsonb)";
        var parameters = new Dictionary<string, object?> { ["fechaInicio"] = fechaInicio, ["fechaFin"] = fechaFin, ["jsonInput"] = jsonCarrito };
        var dataTable = _executeQuery.EjecutarFuncion(query, parameters);
        var lista = new List<FechaNoDisponibleDto>(dataTable.Rows.Count);
        foreach (DataRow fila in dataTable.Rows)
        {
            lista.Add(new FechaNoDisponibleDto
            {
                IdGrupoEquipo = Convert.ToInt32(fila["id_grupo_equipo"]),
                FechaNoDisponible = Convert.ToDateTime(fila["fecha_no_disponible"]),
                CantidadDisponible = Convert.ToInt32(fila["cantidad_disponible"])
            });
        }
        return lista;
    }
}