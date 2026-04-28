using System.Data;
using Npgsql;
public class ComponenteRepository :
    ICrearRepository<CrearComponenteComando>,
    IActualizarRepository<ActualizarComponenteComando>,
    IEliminarRepository<EliminarComponenteComando>,
    IObtenerTodosRepository<CrearComponenteComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public ComponenteRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public void Crear(int idEquipo, CrearComponenteComando comando)
    {
        const string sql = @"INSERT INTO public.componentes (nombre, modelo, tipo, id_equipo, descripcion, precio_referencia, url_data_sheet, estado_eliminado)
                             VALUES (@nombre, @modelo, @tipo, @idEquipo, @descripcion, @precioReferencia, @urlDataSheet, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear componente: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al crear componente: {ex.Message}", "crear", "componente", ex); }
    }

    public void Crear(CrearComponenteComando comando)
    {
        throw new InvalidOperationException("Use Crear(int idEquipo, CrearComponenteComando comando) en su lugar.");
    }

    public void Actualizar(int? idEquipo, ActualizarComponenteComando comando)
    {
        const string sql = @"UPDATE public.componentes SET
            nombre = COALESCE(@nombre, nombre),
            modelo = COALESCE(@modelo, modelo),
            tipo = COALESCE(@tipo, tipo),
            id_equipo = COALESCE(@idEquipo, id_equipo),
            descripcion = COALESCE(@descripcion, descripcion),
            precio_referencia = COALESCE(@precioReferencia, precio_referencia),
            url_data_sheet = COALESCE(@urlDataSheet, url_data_sheet)
            WHERE id_componente = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idEquipo"] = idEquipo ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precioReferencia"] = comando.PrecioReferencia ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar componente: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al actualizar componente: {ex.Message}", "actualizar", "componente", ex); }
    }

    public void Actualizar(ActualizarComponenteComando comando)
    {
        Actualizar(null, comando);
    }

    public void Eliminar(EliminarComponenteComando comando)
    {
        const string sql = @"UPDATE public.componentes SET estado_eliminado = TRUE WHERE id_componente = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar componente: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al eliminar componente: {ex.Message}", "eliminar", "componente", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT c.id_componente, c.nombre AS nombre_componente, c.modelo AS modelo_componente,
            c.tipo AS tipo_componente, c.descripcion AS descripcion_componente,
            c.precio_referencia AS precio_referencia_componente, c.url_data_sheet AS url_data_sheet_equipo,
            ge.nombre AS nombre_equipo, e.codigo_imt AS codigo_imt_equipo
            FROM public.componentes AS c
            INNER JOIN public.equipos AS e ON c.id_equipo = e.id_equipo
            INNER JOIN public.grupos_equipos AS ge ON e.id_grupo_equipo = ge.id_grupo_equipo
            WHERE c.estado_eliminado = FALSE";
        return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
    }

    // --- Métodos auxiliares ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.componentes WHERE id_componente = @id AND estado_eliminado = FALSE)";
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
