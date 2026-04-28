using System.Data;
using Npgsql;

public class MantenimientoRepository :
    ICrearRepository<CrearMantenimientoComando>,
    IEliminarRepository<EliminarMantenimientoComando>,
    IObtenerTodosRepository<CrearMantenimientoComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MantenimientoRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    
    public int CrearMantenimiento(int idEmpresa, CrearMantenimientoComando comando)
    {
        const string sql = @"INSERT INTO public.mantenimientos (fecha_mantenimiento, fecha_final_mantenimiento, id_empresa, costo, descripcion, estado_eliminado)
                             VALUES (@fechaMantenimiento, @fechaFinalMantenimiento, @idEmpresa, @costo, @descripcion, FALSE)
                             RETURNING id_mantenimiento";
        var parametros = new Dictionary<string, object?>
        {
            ["fechaMantenimiento"] = comando.FechaMantenimiento!,
            ["fechaFinalMantenimiento"] = comando.FechaFinalDeMantenimiento!,
            ["idEmpresa"] = idEmpresa,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["descripcion"] = comando.DescripcionMantenimiento ?? (object)DBNull.Value
        };
        try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear mantenimiento: {ex.Message}", ex); }
    }

    public void Crear(CrearMantenimientoComando comando)
    {
        throw new InvalidOperationException("Use CrearMantenimiento(int idEmpresa, CrearMantenimientoComando comando) en su lugar.");
    }

    public void CrearDetalleMantenimiento(int idMantenimiento, int idEquipo, string? tipoMantenimiento, string? descripcionEquipo)
    {
        const string sql = @"INSERT INTO public.detalles_mantenimientos (id_mantenimiento, id_equipo, tipo_mantenimiento, descripcion, estado_eliminado)
                             VALUES (@idMantenimiento, @idEquipo, @tipoMantenimiento, @descripcion, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["idMantenimiento"] = idMantenimiento,
            ["idEquipo"] = idEquipo,
            ["tipoMantenimiento"] = tipoMantenimiento ?? (object)DBNull.Value,
            ["descripcion"] = descripcionEquipo ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear detalle mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear detalle mantenimiento: {ex.Message}", ex); }
    }

    public void Eliminar(EliminarMantenimientoComando comando)
    {
        // Eliminar detalles primero
        const string sqlDetalles = @"UPDATE public.detalles_mantenimientos SET estado_eliminado = TRUE WHERE id_mantenimiento = @id";
        var parametrosDetalles = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sqlDetalles, parametrosDetalles); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar detalles mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar detalles mantenimiento: {ex.Message}", ex); }

        // Eliminar mantenimiento
        const string sql = @"UPDATE public.mantenimientos SET estado_eliminado = TRUE WHERE id_mantenimiento = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar mantenimiento: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar mantenimiento: {ex.Message}", ex); }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT m.id_mantenimiento, m.fecha_mantenimiento, m.fecha_final_mantenimiento,
            em.nombre AS nombre_empresa_mantenimiento, m.costo AS costo_mantenimiento,
            m.descripcion AS descripcion_mantenimiento, e.codigo_imt AS codigo_imt_equipo,
            ge.nombre AS nombre_grupo_equipo, dm.tipo_mantenimiento AS tipo_detalle_mantenimiento,
            dm.descripcion AS descripcion_equipo
            FROM public.mantenimientos AS m
            LEFT JOIN public.empresas_mantenimiento AS em ON m.id_empresa = em.id_empresa_mantenimiento AND em.estado_eliminado = FALSE
            LEFT JOIN public.detalles_mantenimientos AS dm ON m.id_mantenimiento = dm.id_mantenimiento AND dm.estado_eliminado = FALSE
            LEFT JOIN public.equipos AS e ON dm.id_equipo = e.id_equipo AND e.estado_eliminado = FALSE
            LEFT JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo AND ge.estado_eliminado = FALSE
            WHERE m.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener mantenimientos: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener mantenimientos: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.mantenimientos WHERE id_mantenimiento = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerEmpresaIdPorNombre(string nombreEmpresa)
    {
        const string sql = @"SELECT id_empresa_mantenimiento FROM public.empresas_mantenimiento WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreEmpresa };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerEquipoIdPorCodigoImt(int codigoImt)
    {
        const string sql = @"SELECT id_equipo FROM public.equipos WHERE codigo_imt = @codigoImt AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["codigoImt"] = codigoImt };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}