using System.Data;
using Npgsql;

public class AccesorioRepository :
    ICrearRepository<CrearAccesorioComando>,
    IActualizarRepository<ActualizarAccesorioComando>,
    IEliminarRepository<EliminarAccesorioComando>,
    IObtenerTodosRepository<CrearAccesorioComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public AccesorioRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public void Crear(int idEquipo, CrearAccesorioComando comando)
    {
        const string sql = @"INSERT INTO public.accesorios (nombre, descripcion, modelo, url_data_sheet, precio, id_equipo, tipo, estado_eliminado)
                             VALUES (@nombre, @descripcion, @modelo, @urlDataSheet, @precio, @idEquipo, @tipo, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear accesorio: {ex.Message}", ex); }
    }

    public void Crear(CrearAccesorioComando comando)
    {
        throw new InvalidOperationException("Use Crear(int idEquipo, CrearAccesorioComando comando) en su lugar.");
    }

    public void Actualizar(int? idEquipo, ActualizarAccesorioComando comando)
    {
        const string sql = @"UPDATE public.accesorios SET
            nombre = COALESCE(@nombre, nombre),
            descripcion = COALESCE(@descripcion, descripcion),
            modelo = COALESCE(@modelo, modelo),
            url_data_sheet = COALESCE(@urlDataSheet, url_data_sheet),
            precio = COALESCE(@precio, precio),
            id_equipo = COALESCE(@idEquipo, id_equipo),
            tipo = COALESCE(@tipo, tipo)
            WHERE id_accesorio = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar accesorio: {ex.Message}", ex); }
    }

    public void Actualizar(ActualizarAccesorioComando comando)
    {
        Actualizar(null, comando);
    }

    public void Eliminar(EliminarAccesorioComando comando)
    {
        const string sql = @"UPDATE public.accesorios SET estado_eliminado = TRUE WHERE id_accesorio = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar accesorio: {ex.Message}", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT a.id_accesorio, a.nombre AS nombre_accesorio, a.descripcion AS descripcion_accesorio,
            a.modelo AS modelo_accesorio, a.url_data_sheet AS url_data_sheet_accesorio,
            a.precio AS precio_accesorio, a.tipo AS tipo_accesorio,
            e.codigo_imt AS codigo_imt_equipo
            FROM public.accesorios AS a
            INNER JOIN public.equipos AS e ON a.id_equipo = e.id_equipo
            WHERE a.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener accesorios: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener accesorios: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.accesorios WHERE id_accesorio = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
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