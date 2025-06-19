using System.Data;
using Npgsql;

public class CarreraRepository : ICarreraRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;

    public CarreraRepository(IExecuteQuery ejecutarConsulta)
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
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear carrera: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al crear carrera: {ex.Message}", "crear", "carrera", ex);
        }
    }

    public void Eliminar(int id)
    {
        const string sql = @"
            CALL public.eliminar_carrera(
            @id
        )";          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["id"] = id
            });
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al eliminar carrera: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al eliminar carrera: {ex.Message}", "eliminar", "carrera", ex);
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
            ["nombre"] = comando.Nombre ?? (object)DBNull.Value        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al actualizar carrera: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al actualizar carrera: {ex.Message}", "actualizar", "carrera", ex);
        }
    }
    public DataTable ObtenerTodas()
    {
        const string sql = @"
            SELECT * from public.obtener_carreras()
        ";        try
        {
            DataTable dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al obtener carreras: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al obtener carreras: {ex.Message}", "obtener", "carreras", ex);
        }
    }
}