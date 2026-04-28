using System.Data;
using Npgsql;

public class GaveteroRepository :
    ICrearRepository<CrearGaveteroComando>,
    IActualizarRepository<ActualizarGaveteroComando>,
    IEliminarRepository<EliminarGaveteroComando>,
    IObtenerTodosRepository<CrearGaveteroComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public GaveteroRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    
    public void Crear(int idMueble, CrearGaveteroComando comando)
    {
        const string sql = @"INSERT INTO public.gaveteros (nombre, tipo, id_mueble, longitud, profundidad, altura, estado_eliminado)
                             VALUES (@nombre, @tipo, @idMueble, @longitud, @profundidad, @altura, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idMueble"] = idMueble,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear gavetero: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear gavetero: {ex.Message}", ex); }
    }

    // Implementación de la interfaz ICrearRepository (delega con id_mueble = 0 — no usar directamente)
    public void Crear(CrearGaveteroComando comando)
    {
        throw new InvalidOperationException("Use Crear(int idMueble, CrearGaveteroComando comando) en su lugar.");
    }

    public void Actualizar(int? idMueble, ActualizarGaveteroComando comando)
    {
        const string sql = @"UPDATE public.gaveteros SET
            nombre = COALESCE(@nombre, nombre),
            tipo = COALESCE(@tipo, tipo),
            id_mueble = COALESCE(@idMueble, id_mueble),
            longitud = COALESCE(@longitud, longitud),
            profundidad = COALESCE(@profundidad, profundidad),
            altura = COALESCE(@altura, altura)
            WHERE id_gavetero = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["idMueble"] = idMueble ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar gavetero: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar gavetero: {ex.Message}", ex); }
    }

    public void Actualizar(ActualizarGaveteroComando comando)
    {
        Actualizar(null, comando);
    }

    public void Eliminar(EliminarGaveteroComando comando)
    {
        const string sql = @"UPDATE public.gaveteros SET estado_eliminado = TRUE WHERE id_gavetero = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar gavetero: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar gavetero: {ex.Message}", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT g.id_gavetero, g.nombre AS nombre_gavetero, g.tipo AS tipo_gavetero,
            m.nombre AS nombre_mueble, g.longitud AS longitud_gavetero, g.profundidad AS profundidad_gavetero,
            g.altura AS altura_gavetero
            FROM public.gaveteros AS g
            INNER JOIN public.muebles AS m ON g.id_mueble = m.id_mueble
            WHERE g.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener gaveteros: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener gaveteros: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares para la lógica de negocio en el servicio ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivoPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivoPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.gaveteros WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_gavetero <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerMuebleIdPorNombre(string nombreMueble)
    {
        const string sql = @"SELECT id_mueble FROM public.muebles WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreMueble };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }

    public int? ObtenerMuebleIdPorGaveteroId(int gaveteroId)
    {
        const string sql = @"SELECT id_mueble FROM public.gaveteros WHERE id_gavetero = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?> { ["id"] = gaveteroId };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        if (dt.Rows.Count == 0) return null;
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}
