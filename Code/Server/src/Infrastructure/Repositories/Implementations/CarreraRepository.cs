using System.Data;
public class CarreraRepository : ICarreraRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;

    public CarreraRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearCarreraComando comando)
    {
        const string sql = @"
        CALL public.insertar_carrera(
        @nombre
        )";

        var parametros = new Dictionary<string, object?>
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
            throw new Exception($"Error en BD al crear carrera: {innerError}. SQL: {sql}. Parámetros: nombre={comando.Nombre}", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_carrera(
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
            throw new Exception($"Error en BD al eliminar carrera: {innerError}. SQL: {sql}. Parámetros: id={id}", ex);
        }
    }

    public void Actualizar(ActualizarCarreraComando comando)
    {
        const string sql = @"
            CALL public.actualizar_carrera(
                @id,
                @nombre
            )";

        var parametros = new Dictionary<string, object?>
        {
            ["id"] = comando.Id,
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value
        };
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al actualizar carrera: {innerError}. SQL: {sql}. Parámetros: id={comando.Id}, nombre={comando.Nombre}", ex);
        }
    }
    public DataTable ObtenerTodas()
    {
        const string sql = @"
            SELECT * from public.obtener_carreras()
        ";
        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            var innerError = ex.InnerException?.Message ?? ex.Message;
            throw new Exception($"Error en BD al obtener carreras: {innerError}. SQL: {sql}", ex);
        }
    }
}