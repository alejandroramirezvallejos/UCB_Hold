using System.Data;
using Npgsql;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public UsuarioRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;
    public void Crear(CrearUsuarioComando comando)
    {
        const string sql = @"CALL public.insertar_usuario(@carnet,@nombre,@apellidoPaterno,@apellidoMaterno,@rol::tipo_usuario,@email,@contrasena,@carrera,@telefono,@telefonoReferencia,@nombreReferencia,@emailReferencia)";
        var parametros = new Dictionary<string, object?>
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
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al crear usuario: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al crear usuario: {ex.Message}", "crear", "usuario", ex); }
    }
    public void Actualizar(ActualizarUsuarioComando comando)
    {
        const string sql = @"CALL public.actualizar_usuario(@carnet,@nombre,@apellidoPaterno,@apellidoMaterno,@email,@contrasena,@rol::tipo_usuario,@carrera,@telefono,@telefonoReferencia,@nombreReferencia,@emailReferencia)";
        var parametros = new Dictionary<string, object?>
        {
            ["carnet"] = comando.Carnet,
            ["nombre"] = string.IsNullOrEmpty(comando.Nombre) ? (object)DBNull.Value : comando.Nombre,
            ["apellidoPaterno"] = string.IsNullOrEmpty(comando.ApellidoPaterno) ? (object)DBNull.Value : comando.ApellidoPaterno,
            ["apellidoMaterno"] = string.IsNullOrEmpty(comando.ApellidoMaterno) ? (object)DBNull.Value : comando.ApellidoMaterno,
            ["email"] = string.IsNullOrEmpty(comando.Email) ? (object)DBNull.Value : comando.Email,
            ["contrasena"] = string.IsNullOrEmpty(comando.Contrasena) ? (object)DBNull.Value : comando.Contrasena,
            ["rol"] = string.IsNullOrEmpty(comando.Rol) ? (object)DBNull.Value : comando.Rol,
            ["carrera"] = string.IsNullOrEmpty(comando.NombreCarrera) ? (object)DBNull.Value : comando.NombreCarrera,
            ["telefono"] = string.IsNullOrEmpty(comando.Telefono) ? (object)DBNull.Value : comando.Telefono,
            ["telefonoReferencia"] = string.IsNullOrEmpty(comando.TelefonoReferencia) ? (object)DBNull.Value : comando.TelefonoReferencia,
            ["nombreReferencia"] = string.IsNullOrEmpty(comando.NombreReferencia) ? (object)DBNull.Value : comando.NombreReferencia,
            ["emailReferencia"] = string.IsNullOrEmpty(comando.EmailReferencia) ? (object)DBNull.Value : comando.EmailReferencia
        };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al actualizar usuario: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al actualizar usuario: {ex.Message}", "actualizar", "usuario", ex); }
    }
    public void Eliminar(string carnet)
    {
        const string sql = @"CALL public.eliminar_usuario(@carnet)";
        var parametros = new Dictionary<string, object?> { ["carnet"] = carnet };
        try { _ejecutarConsulta.EjecutarSpNR(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al eliminar usuario: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error en repositorio al eliminar usuario: {ex.Message}", "eliminar", "usuario", ex); }
    }
    public DataTable ObtenerTodos()
    {
        const string sql = @"SELECT * from public.obtener_usuarios()";
        try { return _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>()); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener usuarios: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener usuarios: {ex.Message}", ex); }
    }
    public DataTable? ObtenerPorEmailYContrasena(string email, string contrasena)
    {
        const string sql = @"SELECT * from public.obtener_usuario_iniciar_sesion(@email,@contrasena)";
        var parametros = new Dictionary<string, object?>
        {
            ["email"] = email,
            ["contrasena"] = contrasena
        };
        try {
            var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
            if (dt.Rows.Count == 0) return null;
            return dt;
        } catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener usuario: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener usuario: {ex.Message}", ex); }
    }

    public DataTable ObtenerPorCarnets(List<string> carnets)
    {
        if (carnets == null || !carnets.Any()) return new DataTable();
        
        const string sql = @"SELECT carnet, nombre, apellido_paterno FROM public.usuarios WHERE carnet = ANY(@carnets)";
        var parametros = new Dictionary<string, object?> { ["carnets"] = carnets };
        
        try { return _ejecutarConsulta.EjecutarFuncion(sql, parametros); }
        catch (NpgsqlException ex) { throw new ErrorDataBase($"Error de base de datos al obtener usuarios por carnets: {ex.Message}", ex.SqlState, null, ex); }
        catch (Exception ex) { throw new ErrorRepository($"Error del repositorio al obtener usuarios por carnets: {ex.Message}", ex); }
    }
}