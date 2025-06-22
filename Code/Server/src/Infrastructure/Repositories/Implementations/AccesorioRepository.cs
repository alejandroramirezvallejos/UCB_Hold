using System.Data;
using Npgsql;

public class AccesorioRepository : IAccesorioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public AccesorioRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    public void Crear(CrearAccesorioComando comando)
    {
        const string sql = @"CALL public.insertar_accesorios(@nombre,@modelo,@tipo,@codigoImt,@descripcion,@precio,@urlDataSheet)";
        var parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"] = comando.CodigoIMT,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al crear accesorio: {ex.Message}", "crear", "accesorio", ex); }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT * from public.obtener_accesorios()";
        return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
    }
    public void Actualizar(ActualizarAccesorioComando comando)
    {
        const string sql = @"CALL public.actualizar_accesorio(@id,@nombre,@modelo,@tipo,@codigoImt,@descripcion,@precio,@urlDataSheet)";
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["tipo"] = comando.Tipo ?? (object)DBNull.Value,
            ["codigoImt"] = comando.CodigoIMT ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["precio"] = comando.Precio ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al actualizar accesorio: {ex.Message}", "actualizar", "accesorio", ex); }
    }
    public void Eliminar(int id)
    {
        const string sql = @"CALL public.eliminar_accesorio(@id)";
        var parametros = new Dictionary<string, object?> { ["id"] = id };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar accesorio: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al eliminar accesorio: {ex.Message}", "eliminar", "accesorio", ex); }
    }
}