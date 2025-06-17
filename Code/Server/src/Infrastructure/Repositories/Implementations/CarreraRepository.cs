using System.Data;
using Npgsql;
using Shared.Common;

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
        };        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "carrera", parametros);
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
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?> { ["id"] = id };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "carrera", parametros);
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
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "carrera", parametros);
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
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "carreras", null);
        }
    }
}