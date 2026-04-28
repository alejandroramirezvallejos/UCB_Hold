using System.Data;
using Npgsql;

public class MuebleRepository :
    ICrearRepository<CrearMuebleComando>,
    IActualizarRepository<ActualizarMuebleComando>,
    IEliminarRepository<EliminarMuebleComando>,
    IObtenerTodosRepository<CrearMuebleComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public MuebleRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public void Crear(CrearMuebleComando comando)
    {
        const string sql = @"INSERT INTO public.muebles (nombre, tipo, costo, ubicacion, longitud, profundidad, altura, estado_eliminado)
                             VALUES (@nombre, @tipo, @costo, @ubicacion, @longitud, @profundidad, @altura, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear mueble: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al crear mueble: {ex.Message}", ex); }
    }

    public void Actualizar(ActualizarMuebleComando comando)
    {
        const string sql = @"UPDATE public.muebles SET
            nombre = COALESCE(@nombre, nombre),
            tipo = COALESCE(@tipo, tipo),
            costo = COALESCE(@costo, costo),
            ubicacion = COALESCE(@ubicacion, ubicacion),
            longitud = COALESCE(@longitud, longitud),
            profundidad = COALESCE(@profundidad, profundidad),
            altura = COALESCE(@altura, altura)
            WHERE id_mueble = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["costo"] = comando.Costo ?? (object)DBNull.Value,
            ["ubicacion"] = comando.Ubicacion ?? (object)DBNull.Value,
            ["longitud"] = comando.Longitud ?? (object)DBNull.Value,
            ["profundidad"] = comando.Profundidad ?? (object)DBNull.Value,
            ["altura"] = comando.Altura ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar mueble: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al actualizar mueble: {ex.Message}", ex); }
    }

    public void Eliminar(EliminarMuebleComando comando)
    {
        const string sql = @"UPDATE public.muebles SET estado_eliminado = TRUE WHERE id_mueble = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar mueble: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al eliminar mueble: {ex.Message}", ex); }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT m.id_mueble, m.nombre AS nombre_mueble, m.numero_gaveteros AS numero_gaveteros_mueble,
            m.ubicacion AS ubicacion_mueble, m.tipo AS tipo_mueble, m.costo AS costo_mueble,
            m.longitud AS longitud_mueble, m.profundidad AS profundidad_mueble, m.altura AS altura_mueble
            FROM public.muebles AS m WHERE m.estado_eliminado = FALSE";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener muebles: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener muebles: {ex.Message}", ex); }
    }

    // --- Métodos auxiliares para la lógica de negocio en el servicio ---

    public bool ExisteActivoPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.muebles WHERE id_mueble = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public void ActualizarNumeroGaveteros(int idMueble, int incremento)
    {
        const string sql = @"UPDATE public.muebles SET numero_gaveteros = GREATEST(0, COALESCE(numero_gaveteros, 0) + @incremento) WHERE id_mueble = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = idMueble, ["incremento"] = incremento };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}