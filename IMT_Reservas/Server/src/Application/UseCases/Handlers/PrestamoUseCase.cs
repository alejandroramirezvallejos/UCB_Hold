using System.Data;

public class PrestamoUseCase : ICrearPrestamoComando, IObtenerPrestamoConsulta, 
                               IActualizarPrestamoComando, IEliminarPrestamoComando
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public PrestamoUseCase(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public PrestamoDto Handle(CrearPrestamoComando comando)
    {
        const string sql = @"
            INSERT INTO public.prestamos
            (fecha_solicitud, fecha_prestamo, fecha_devolucion,
             fecha_devolucion_esperada, observacion, estado_prestamo,
             carnet_usuario, id_equipo, estado_eliminado)
            VALUES
            (@fechaSolicitud, @fechaPrestamo, @fechaDevolucion,
             @fechaDevolucionEsperada, @observacion, @estadoPrestamo,
             @carnetUsuario, @equipoId, false)
            RETURNING *;";

        var parametros = new Dictionary<string, object>
        {
            ["fechaSolicitud"]          = comando.FechaSolicitud,
            ["fechaPrestamo"]           = comando.FechaPrestamo,
            ["fechaDevolucion"]         = comando.FechaDevolucion,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada,
            ["observacion"]             = comando.Observacion ?? (object)DBNull.Value,
            ["estadoPrestamo"]          = comando.EstadoPrestamo,
            ["carnetUsuario"]           = comando.CarnetUsuario,
            ["equipoId"]                = comando.EquipoId
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return MapearFilaADto(dt.Rows[0]);
    }

    public PrestamoDto? Handle(ObtenerPrestamoConsulta consulta)
    {
        const string sql = @"
            SELECT * FROM public.prestamos
            WHERE id_prestamo = @id";

        var parametros = new Dictionary<string, object> { ["id"] = consulta.Id };
        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }

    public PrestamoDto? Handle(ActualizarPrestamoComando comando)
    {
        const string sql = @"
            UPDATE public.prestamos
            SET
              fecha_solicitud = @fechaSolicitud,
              fecha_prestamo = @fechaPrestamo,
              fecha_devolucion = @fechaDevolucion,
              fecha_devolucion_esperada = @fechaDevolucionEsperada,
              observacion = @observacion,
              estado_prestamo = @estadoPrestamo,
              carnet_usuario = @carnetUsuario,
              id_equipo = @equipoId
            WHERE id_prestamo = @id
            RETURNING *;";

        var parametros = new Dictionary<string, object>
        {
            ["id"]                      = comando.Id,
            ["fechaSolicitud"]          = comando.FechaSolicitud,
            ["fechaPrestamo"]           = comando.FechaPrestamo,
            ["fechaDevolucion"]         = comando.FechaDevolucion,
            ["fechaDevolucionEsperada"] = comando.FechaDevolucionEsperada,
            ["observacion"]             = comando.Observacion ?? (object)DBNull.Value,
            ["estadoPrestamo"]          = comando.EstadoPrestamo,
            ["carnetUsuario"]           = comando.CarnetUsuario,
            ["equipoId"]                = comando.EquipoId
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }

    public bool Handle(EliminarPrestamoComando comando)
    {
        const string sql = @"
            UPDATE public.prestamos
            SET estado_eliminado = true
            WHERE id_prestamo = @id";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object> { ["id"] = comando.Id });
        return true;
    }

    private static PrestamoDto MapearFilaADto(DataRow fila)
    {
        return new PrestamoDto
        {
            Id                      = Convert.ToInt32(fila["id_prestamo"]),
            FechaSolicitud          = Convert.ToDateTime(fila["fecha_solicitud"]),
            FechaPrestamo           = Convert.ToDateTime(fila["fecha_prestamo"]),
            FechaDevolucion         = Convert.ToDateTime(fila["fecha_devolucion"]),
            FechaDevolucionEsperada = Convert.ToDateTime(fila["fecha_devolucion_esperada"]),
            Observacion             = fila["observacion"] as string,
            EstadoPrestamo          = fila["estado_prestamo"].ToString() ?? string.Empty,
            CarnetUsuario           = fila["carnet_usuario"].ToString() ?? string.Empty,
            EquipoId                = Convert.ToInt32(fila["id_equipo"]),
            EstaEliminado           = Convert.ToBoolean(fila["estado_eliminado"])
        };
    }
}