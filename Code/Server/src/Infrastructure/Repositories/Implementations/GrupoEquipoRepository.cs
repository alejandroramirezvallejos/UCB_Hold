using System.Data;

public class GrupoEquipoRepository : IGrupoEquipoRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public GrupoEquipoRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }
    public void Crear(CrearGrupoEquipoComando comando)
    {
        const string sql = @"
            CALL public.insertar_grupo_equipo(
	        @nombre,
	        @modelo,
	        @marca,
	        @descripcion,
	        @nombreCategoria,
	        @urlDataSheet,
	        @urlImagen
            )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["nombreCategoria"] = comando.NombreCategoria ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear grupo de equipo: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}, marca={comando.Marca}", ex);
        }
    }

    public DataTable? ObtenerPorId(int id)
    {
        const string sql = @"
            SELECT * from public.obtener_grupo_equipo_especifico_por_id(
	        @id
            )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener grupo de equipo por ID: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }

    public DataTable ObtenerPorNombreYCategoria(string? nombre, string? categoria)
    {
        const string sql = @"
            SELECT * from public.obtener_grupos_equipos_por_nombre_y_categoria(
	        @nombre,
	        @categoria    
            )";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>
            {
                ["nombre"] = nombre ?? (object)DBNull.Value,
                ["categoria"] = categoria ?? (object)DBNull.Value
            });
            
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener grupos de equipos por nombre y categoría: {innerError}. SQL: {sql}. Parámetros: nombre={nombre}, categoria={categoria}", ex);
        }
    }

    public void Actualizar(ActualizarGrupoEquipoComando comando)
    {
        const string sql = @"
        CALL public.actualizar_grupo_equipo(
	    @id,
	    @nombre,
	    @modelo,
	    @marca,
	    @descripcion,
	    @nombreCategoria,
	    @urlDataSheet,
	    @urlImagen
        )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value,
            ["modelo"] = comando.Modelo ?? (object)DBNull.Value,
            ["marca"] = comando.Marca ?? (object)DBNull.Value,
            ["descripcion"] = comando.Descripcion ?? (object)DBNull.Value,
            ["nombreCategoria"] = comando.NombreCategoria ?? (object)DBNull.Value,
            ["urlDataSheet"] = comando.UrlDataSheet ?? (object)DBNull.Value,
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar grupo de equipo: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_grupo_equipo(
	        @id
            )";
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al eliminar grupo de equipo: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_grupos_equipos()
        ";

        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener grupos de equipos: {innerError}. SQL: {sql}", ex);
        }
    }
}