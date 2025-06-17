using System.Data;
using Npgsql;
using Shared.Common;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public CategoriaRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearCategoriaComando comando)
    {
        const string sql = @"
            CALL public.insertar_categoria(
	        @nombre
            )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["nombre"] = comando.Nombre
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "categoría", parametros);
        }
    }    
    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_categorias()
        ";

        DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt;
    }

    public void Actualizar(ActualizarCategoriaComando comando)
    {
        const string sql = @"
        CALL public.actualizar_categoria(
		@id,
		@nombre
        )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"]     = comando.Id,
            ["nombre"] = comando.Nombre == null ? null : comando.Nombre
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "categoría", parametros);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_categoria(
	    @id
        )";        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "categoría", parametros);
        }
    }
}