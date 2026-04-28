using System.Data;
using Npgsql;

public class CategoriaRepository :
    ICrearRepository<CrearCategoriaComando>,
    IActualizarRepository<ActualizarCategoriaComando>,
    IEliminarRepository<EliminarCategoriaComando>,
    IObtenerTodosRepository<CrearCategoriaComando, DataTable>
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public CategoriaRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    
    public void Crear(CrearCategoriaComando comando)
    {
        const string sql = @"INSERT INTO public.categorias (nombre, estado_eliminado) VALUES (@nombre, FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = comando.Nombre };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear categoría: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al crear categoría: {ex.Message}", "crear", "categoría", ex); }
    }
    
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT c.id_categoria, c.nombre AS categoria FROM public.categorias AS c WHERE c.estado_eliminado = FALSE";
        return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
    }
    
    public void Actualizar(ActualizarCategoriaComando comando)
    {
        const string sql = @"UPDATE public.categorias SET nombre = COALESCE(@nombre, nombre) WHERE id_categoria = @id AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar categoría: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al actualizar categoría: {ex.Message}", "actualizar", "categoría", ex); }
    }
    
    public void Eliminar(EliminarCategoriaComando comando)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = TRUE WHERE id_categoria = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = comando.Id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar categoría: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al eliminar categoría: {ex.Message}", "eliminar", "categoría", ex); }
    }

    // --- Métodos auxiliares para la lógica de negocio en el servicio ---

    public bool ExisteActivaPorId(int id)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE id_categoria = @id AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombre(string nombre)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ExisteActivaPorNombreExcluyendoId(string nombre, int idExcluir)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE AND id_categoria <> @idExcluir)";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre, ["idExcluir"] = idExcluir };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public bool ReactivarEliminadaPorNombre(string nombre)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = FALSE WHERE nombre = @nombre AND estado_eliminado = TRUE";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombre };
        try 
        { 
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
            var checkSql = @"SELECT EXISTS(SELECT 1 FROM public.categorias WHERE nombre = @nombre AND estado_eliminado = FALSE)";
            var dt = _ejecutarConsulta.EjecutarFuncion(checkSql, parametros);
            return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
        }
        catch { return false; }
    }

    public void EliminarLogicamentePorId(int id)
    {
        const string sql = @"UPDATE public.categorias SET estado_eliminado = TRUE WHERE id_categoria = @id";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
    }
}