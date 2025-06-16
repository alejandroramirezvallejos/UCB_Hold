using System.Data;

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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al crear categoría: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}", ex);
        }
    }

    public DataTable ObtenerTodos()
    {
        const string sql = @"
            SELECT * from public.obtener_categorias()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener categorías: {innerError}. SQL: {sql}", ex);
        }
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
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar categoría: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
        CALL public.eliminar_categoria(
	    @id
        )";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["id"] = id
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al eliminar categoría: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }
}