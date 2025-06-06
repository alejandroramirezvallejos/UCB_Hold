using System.Data;

public class AccesorioRepository : IAccesorioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public AccesorioRepository(IExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public AccesorioDto Crear(CrearAccesorioComando comando)
    {
        const string sql = @"
            INSERT INTO public.accesorios
              (nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado)
            VALUES (@nombre, @descripcion, @modelo, @url, @precio, @equipoId, @tipo, false)
            RETURNING id_accesorio, nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"]      = comando.Nombre,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"]      = comando.Modelo ?? (object)DBNull.Value,
            ["url"]         = comando.Url ?? (object)DBNull.Value,
            ["precio"]      = comando.Precio ?? (object)DBNull.Value,
            ["equipoId"]    = comando.EquipoId,
            ["tipo"]        = comando.Tipo ?? (object)DBNull.Value
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return MapearFilaADto(dt.Rows[0]);
    }

    public AccesorioDto? ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT id_accesorio, nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado
              FROM public.accesorios
             WHERE id_accesorio = @id
               AND estado_eliminado = false;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }

    public List<AccesorioDto> ObtenerTodos()
    {
        const string sql = @"
            SELECT id_accesorio, nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado
              FROM public.accesorios
             WHERE estado_eliminado = false;
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        var lista = new List<AccesorioDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
    }

    public List<AccesorioDto> ObtenerPorEquipoId(int equipoId)
    {
        const string sql = @"
            SELECT id_accesorio, nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado
              FROM public.accesorios
             WHERE id_equipo = @equipoId
               AND estado_eliminado = false;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["equipoId"] = equipoId
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        var lista = new List<AccesorioDto>(dt.Rows.Count);
        foreach (DataRow row in dt.Rows)
            lista.Add(MapearFilaADto(row));
        return lista;
    }

    public AccesorioDto? Actualizar(ActualizarAccesorioComando comando)
    {
        const string sql = @"
            UPDATE public.accesorios
               SET nombre      = @nombre,
                   descripcion = @descripcion,
                   modelo      = @modelo,
                   url         = @url,
                   precio      = @precio,
                   id_equipo   = @equipoId,
                   tipo        = @tipo
             WHERE id_accesorio = @id;

            SELECT id_accesorio, nombre, descripcion, modelo, url, precio, id_equipo, tipo, estado_eliminado
              FROM public.accesorios
             WHERE id_accesorio = @id;
        ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]          = comando.Id,
            ["nombre"]      = comando.Nombre,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"]      = comando.Modelo ?? (object)DBNull.Value,
            ["url"]         = comando.Url ?? (object)DBNull.Value,
            ["precio"]      = comando.Precio ?? (object)DBNull.Value,
            ["equipoId"]    = comando.EquipoId,
            ["tipo"]        = comando.Tipo ?? (object)DBNull.Value
        };

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return MapearFilaADto(dt.Rows[0]);
    }

    public bool Eliminar(int id)
    {
        const string sql = @"
            UPDATE public.accesorios
               SET estado_eliminado = true
             WHERE id_accesorio = @id;
        ";

        _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
        {
            ["id"] = id
        });
        return true;
    }

    private static AccesorioDto MapearFilaADto(DataRow fila)
    {
        return new AccesorioDto
        {
            Id            = Convert.ToInt32(fila["id_accesorio"]),
            Nombre        = fila["nombre"]?.ToString() ?? string.Empty,
            Descripcion   = fila["descripcion"] == DBNull.Value ? null : fila["descripcion"].ToString(),
            Modelo        = fila["modelo"] == DBNull.Value ? null : fila["modelo"].ToString(),
            Url           = fila["url"] == DBNull.Value ? null : fila["url"].ToString(),
            Precio        = fila["precio"] == DBNull.Value ? null : Convert.ToDouble(fila["precio"]),
            EquipoId      = Convert.ToInt32(fila["id_equipo"]),
            Tipo          = fila["tipo"] == DBNull.Value ? null : fila["tipo"].ToString(),
            EstaEliminado = Convert.ToBoolean(fila["estado_eliminado"])
        };
    }
}