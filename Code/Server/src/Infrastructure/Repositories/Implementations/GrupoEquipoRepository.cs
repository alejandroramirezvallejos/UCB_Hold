using System.Data;
using Npgsql;
using Shared.Common;

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
            ["urlImagen"] = comando.UrlImagen ?? (object)DBNull.Value        };
        
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "grupo de equipo", parametros);
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
        };        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt;
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "grupo de equipo", parametros);
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
        }        catch (Exception ex)
        {
            var parametrosConsulta = new Dictionary<string, object?> { ["nombre"] = nombre, ["categoria"] = categoria };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "grupos de equipos", parametrosConsulta);
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
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "grupo de equipo", parametros);
        }
    }

    public void Eliminar(int id)
    {        const string sql = @"
            CALL public.eliminar_grupo_equipo(
	        @id
            )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "grupo de equipo", parametros);
        }
    }      public DataTable ObtenerTodos()
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
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "grupos de equipos", new Dictionary<string, object?>());
        }
    }
}