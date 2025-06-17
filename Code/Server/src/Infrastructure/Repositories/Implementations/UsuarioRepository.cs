using System.Data;
using Npgsql;
using Shared.Common;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ExecuteQuery _ejecutarConsulta;
    public UsuarioRepository(ExecuteQuery ejecutarConsulta)
    {
        _ejecutarConsulta = ejecutarConsulta;
    }

    public void Crear(CrearUsuarioComando comando)
    {
        const string sql = @"
        CALL public.insertar_usuario(
	    @carnet,
	    @nombre,
	    @apellidoPaterno,
	    @apellidoMaterno,
	    @email,
	    @contrasena,
	    @carrera,
	    @telefono,
	    @telefonoReferencia,
	    @nombreReferencia,
	    @emailReferencia
	    )";
        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["carnet"] = comando.Carnet,
            ["nombre"] = comando.Nombre,
            ["apellidoPaterno"] = comando.ApellidoPaterno,
            ["apellidoMaterno"] = comando.ApellidoMaterno,
            ["email"] = comando.Email,
            ["contrasena"] = comando.Contrasena,
            ["carrera"] = comando.NombreCarrera ?? (object)DBNull.Value,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["telefonoReferencia"] = comando.TelefonoReferencia ?? (object)DBNull.Value,
            ["nombreReferencia"] = comando.NombreReferencia ?? (object)DBNull.Value,
            ["emailReferencia"] = comando.EmailReferencia ?? (object)DBNull.Value
        };          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "usuario", parametros);
        }
    }

    public void Actualizar(ActualizarUsuarioComando comando)
    {
        const string sql = @"
        CALL public.actualizar_usuario(
	    @carnet,
	    @nombre,
	    @apellidoPaterno,
	    @apellidoMaterno,
	    @email,
	    @contrasena,
	    @rol,
	    @carrera,
	    @telefono,
	    @telefonoReferencia,
	    @nombreReferencia,
	    @emailReferencia
        );";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>
        {
            ["carnet"] = comando.Carnet,
            ["nombre"] = comando.Nombre,
            ["apellidoPaterno"] = comando.ApellidoPaterno,
            ["apellidoMaterno"] = comando.ApellidoMaterno,
            ["email"] = comando.Email,
            ["contrasena"] = comando.Contrasena,
            ["rol"] = comando.Rol,
            ["carrera"] = comando.NombreCarrera,
            ["telefono"] = comando.Telefono,
            ["telefonoReferencia"] = comando.TelefonoReferencia,
            ["nombreReferencia"] = comando.NombreReferencia,
            ["emailReferencia"] = comando.EmailReferencia
        };          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "usuario", parametros);
        }
    }

    public void Eliminar(string carnet)
    {        const string sql = @"
        CALL public.eliminar_usuario(
	    @carnet
        )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["carnet"] = carnet
        };
          try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "usuario", parametros);
        }
    }      public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_usuarios()";

        try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "usuarios", new Dictionary<string, object?>());
        }
    }
    public DataTable? ObtenerPorEmailYContrasena(string email, string contrasena)
    {
        const string sql = @"
        SELECT * from public.obtener_usuario_iniciar_sesion(
            @email,
            @contrasena
        )";
        
        var parametros = new Dictionary<string, object?>
        {
            ["email"] = email,
            ["contrasena"] = contrasena
        };
          try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);

            if (dt.Rows.Count == 0)
                return null;

            return dt;
        }
        catch (Exception ex)
        {
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "obtener", "usuario", parametros);
        }
    }
}