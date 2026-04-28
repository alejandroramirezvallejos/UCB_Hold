using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Shared.Common;
using System.Data;
using System.Text.Json;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarritoRepository
{
    private readonly IExecuteQuery _executeQuery;
    public CarritoRepository(IExecuteQuery executeQuery) => _executeQuery = executeQuery;

    public IEnumerable<FechaNoDisponibleDto> ObtenerFechasNoDisponibles(DateTime fechaInicio, DateTime fechaFin, Dictionary<int, int> carrito)
    {
        // Para cada grupo y cada fecha, calcular cuántos equipos están disponibles
        // Un equipo no está disponible si tiene un préstamo activo/pendiente/aprobado que se solapa
        var resultado = new List<FechaNoDisponibleDto>();

        foreach (var (idGrupoEquipo, cantidadSolicitada) in carrito)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var fechaIniciodia = fecha;
                var fechaFinDia = fecha.AddDays(1);

                // Contar equipos totales operativos del grupo
                var sqlTotal = @"SELECT COUNT(*) FROM public.equipos 
                    WHERE id_grupo_equipo = @idGrupoEquipo AND estado_eliminado = FALSE AND estado_equipo = 'operativo'";
                var dtTotal = _executeQuery.EjecutarFuncion(sqlTotal, new Dictionary<string, object?> { ["idGrupoEquipo"] = idGrupoEquipo });
                var totalEquipos = Convert.ToInt32(dtTotal.Rows[0][0]);

                // Contar equipos ocupados en esta fecha
                var sqlOcupados = @"SELECT COUNT(DISTINCT dp.id_equipo) 
                    FROM public.detalles_prestamos AS dp
                    INNER JOIN public.prestamos AS p ON dp.id_prestamo = p.id_prestamo
                    INNER JOIN public.equipos AS e ON dp.id_equipo = e.id_equipo
                    WHERE e.id_grupo_equipo = @idGrupoEquipo
                    AND p.estado_eliminado = FALSE AND dp.estado_eliminado = FALSE
                    AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                    AND p.fecha_prestamo_esperada < @fechaFinDia
                    AND p.fecha_devolucion_esperada > @fechaInicioDia";
                var dtOcupados = _executeQuery.EjecutarFuncion(sqlOcupados, new Dictionary<string, object?>
                {
                    ["idGrupoEquipo"] = idGrupoEquipo,
                    ["fechaInicioDia"] = fechaIniciodia,
                    ["fechaFinDia"] = fechaFinDia
                });
                var ocupados = Convert.ToInt32(dtOcupados.Rows[0][0]);

                var disponibles = totalEquipos - ocupados;

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

    public IEnumerable<DisponibilidadEquipoDto> ObtenerDisponibilidadEquiposPorFechasYGrupos(DateTime fechaInicio, DateTime fechaFin, int[] arrayIds)
    {
        var resultado = new List<DisponibilidadEquipoDto>();

        foreach (var idGrupoEquipo in arrayIds)
        {
            for (var fecha = fechaInicio.Date; fecha <= fechaFin.Date; fecha = fecha.AddDays(1))
            {
                var fechaIniciodia = fecha;
                var fechaFinDia = fecha.AddDays(1);

                // Contar equipos totales operativos del grupo
                var sqlTotal = @"SELECT COUNT(*) FROM public.equipos 
                    WHERE id_grupo_equipo = @idGrupoEquipo AND estado_eliminado = FALSE AND estado_equipo = 'operativo'";
                var dtTotal = _executeQuery.EjecutarFuncion(sqlTotal, new Dictionary<string, object?> { ["idGrupoEquipo"] = idGrupoEquipo });
                var totalEquipos = Convert.ToInt32(dtTotal.Rows[0][0]);

                // Contar equipos ocupados en esta fecha
                var sqlOcupados = @"SELECT COUNT(DISTINCT dp.id_equipo) 
                    FROM public.detalles_prestamos AS dp
                    INNER JOIN public.prestamos AS p ON dp.id_prestamo = p.id_prestamo
                    INNER JOIN public.equipos AS e ON dp.id_equipo = e.id_equipo
                    WHERE e.id_grupo_equipo = @idGrupoEquipo
                    AND p.estado_eliminado = FALSE AND dp.estado_eliminado = FALSE
                    AND p.estado_prestamo IN ('pendiente', 'aprobado', 'activo')
                    AND p.fecha_prestamo_esperada < @fechaFinDia
                    AND p.fecha_devolucion_esperada > @fechaInicioDia";
                var dtOcupados = _executeQuery.EjecutarFuncion(sqlOcupados, new Dictionary<string, object?>
                {
                    ["idGrupoEquipo"] = idGrupoEquipo,
                    ["fechaInicioDia"] = fechaIniciodia,
                    ["fechaFinDia"] = fechaFinDia
                });
                var ocupados = Convert.ToInt32(dtOcupados.Rows[0][0]);

                resultado.Add(new DisponibilidadEquipoDto
                {
                    Fecha = fecha,
                    IdGrupoEquipo = idGrupoEquipo,
                    CantidadDisponible = totalEquipos - ocupados
                });
            }
        }
        return resultado;
    }
}
