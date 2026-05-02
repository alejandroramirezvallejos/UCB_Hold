using IMT_Reservas.Server.Application.Features.Carrito.Dtos;
using System.Data;
using Ardalis.Result;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarritoRepository
{
    private readonly IExecuteQuery _executeQuery;
    public CarritoRepository(IExecuteQuery executeQuery) => _executeQuery = executeQuery;

    public IEnumerable<FechaNoDisponibleDto> GetUnavailableDates(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito)
    {
        var resultado = new List<FechaNoDisponibleDto>();
        var diasSolicitados = (fechaFin.Date - fechaInicio.Date).Days;

        foreach (var (idGrupoEquipo, cantidadSolicitada) in carrito)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var disponibles = GetAvailableEquipmentCount(idGrupoEquipo, fecha, diasSolicitados);

                if (disponibles < cantidadSolicitada)
                {
                    resultado.Add(new FechaNoDisponibleDto
                    {
                        IdGrupoEquipo = idGrupoEquipo,
                        FechaNoDisponible = fecha,
                        CantidadDisponible = disponibles
                    });
                }
            }
        }
        return resultado;
    }

    public IEnumerable<DisponibilidadEquipoDto> GetAvailability(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds)
    {
        var resultado = new List<DisponibilidadEquipoDto>();
        var diasSolicitados = (fechaFin.Date - fechaInicio.Date).Days;

        foreach (var idGrupoEquipo in arrayIds)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var disponibles = GetAvailableEquipmentCount(idGrupoEquipo, fecha, diasSolicitados);

                resultado.Add(new DisponibilidadEquipoDto
                {
                    Fecha = fecha,
                    IdGrupoEquipo = idGrupoEquipo,
                    CantidadDisponible = disponibles
                });
            }
        }
        return resultado;
    }

    private int GetAvailableEquipmentCount(int idGrupoEquipo, DateTime fecha, int diasSolicitados)
    {
        const string sqlDisponibles = @"SELECT COUNT(*)
            FROM (
                SELECT e.id_equipo
                FROM public.equipos e
                WHERE e.id_grupo_equipo = @idGrupoEquipo
                  AND e.estado_eliminado = FALSE
                  AND e.estado_equipo = 'operativo'
                  AND @diasSolicitados <= e.tiempo_max_prestamo

                EXCEPT

                SELECT DISTINCT dp.id_equipo
                FROM public.detalles_prestamos dp
                INNER JOIN public.prestamos p ON dp.id_prestamo = p.id_prestamo
                WHERE p.estado_eliminado = FALSE
                  AND dp.estado_eliminado = FALSE
                  AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                  AND @fechaActual BETWEEN p.fecha_prestamo_esperada::date AND p.fecha_devolucion_esperada::date

                EXCEPT

                SELECT DISTINCT dm.id_equipo
                FROM public.detalles_mantenimientos dm
                INNER JOIN public.mantenimientos m ON dm.id_mantenimiento = m.id_mantenimiento
                WHERE m.estado_eliminado = FALSE
                  AND @fechaActual BETWEEN m.fecha_mantenimiento AND m.fecha_final_mantenimiento
            ) AS equipos_disponibles";

        var parametros = new Dictionary<string, object?>
        {
            ["idGrupoEquipo"] = idGrupoEquipo,
            ["fechaActual"] = fecha,
            ["diasSolicitados"] = diasSolicitados
        };

        var dtDisponibles = _executeQuery.EjecutarFuncion(sqlDisponibles, parametros);
        return Convert.ToInt32(dtDisponibles.Rows[0][0]);
    }
}

