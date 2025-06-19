using System.Data;
using Npgsql;

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
        @rol::tipo_usuario,
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
            ["rol"] = comando.Rol,
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al crear usuario: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al crear usuario: {ex.Message}", "crear", "usuario", ex);
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
	    @rol::tipo_usuario,
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al actualizar usuario: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al actualizar usuario: {ex.Message}", "actualizar", "usuario", ex);
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al eliminar usuario: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error en repositorio al eliminar usuario: {ex.Message}", "eliminar", "usuario", ex);
        }
    }      public DataTable ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_usuarios()";

        try
        {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            return dt;
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al obtener usuarios: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al obtener usuarios: {ex.Message}", ex);
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
        }        catch (NpgsqlException ex)
        {
            throw new ErrorDataBase($"Error de base de datos al obtener usuario: {ex.Message}", ex.SqlState, null, ex);
        }
        catch (Exception ex)
        {
            throw new ErrorRepository($"Error del repositorio al obtener usuario: {ex.Message}", ex);
        }
    }
}