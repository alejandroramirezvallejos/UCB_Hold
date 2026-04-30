using System.Data;
using Ardalis.Result;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public UsuarioRepository(IExecuteQuery ejecutarConsulta) => _ejecutarConsulta = ejecutarConsulta;

    public Result<UsuarioDto?> Crear(int idCarrera, CrearUsuarioComando comando)
    {
        const string sql = @"INSERT INTO public.usuarios (carnet, nombre, apellido_paterno, apellido_materno, rol, email, contrasena, id_carrera, telefono, telefono_referencia, nombre_referencia, email_referencia, estado_eliminado)
                             VALUES (@carnet, @nombre, @apellidoPaterno, @apellidoMaterno, @rol::tipo_usuario, @email, @contrasena, @idCarrera, @telefono, @telefonoReferencia, @nombreReferencia, @emailReferencia, FALSE)";
        var parametros = new Dictionary<string, object?>
        {
            ["carnet"] = comando.Carnet,
            ["nombre"] = comando.Nombre,
            ["apellidoPaterno"] = comando.ApellidoPaterno,
            ["apellidoMaterno"] = comando.ApellidoMaterno,
            ["rol"] = comando.Rol,
            ["email"] = comando.Email,
            ["contrasena"] = comando.Contrasena,
            ["idCarrera"] = idCarrera,
            ["telefono"] = comando.Telefono ?? (object)DBNull.Value,
            ["telefonoReferencia"] = comando.TelefonoReferencia ?? (object)DBNull.Value,
            ["nombreReferencia"] = comando.NombreReferencia ?? (object)DBNull.Value,
            ["emailReferencia"] = comando.EmailReferencia ?? (object)DBNull.Value
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new UsuarioDto { Carnet = comando.Carnet, Nombre = comando.Nombre };
        return Result<UsuarioDto?>.Created(dto);
    }

    public Result<UsuarioDto?> Crear(CrearUsuarioComando comando)
        => Result<UsuarioDto?>.Error("Use Crear(int idCarrera, CrearUsuarioComando comando)");

    public Result<UsuarioDto?> Actualizar(int? idCarrera, ActualizarUsuarioComando comando)
    {
        const string sql = @"UPDATE public.usuarios SET
            nombre = COALESCE(@nombre, nombre),
            apellido_paterno = COALESCE(@apellidoPaterno, apellido_paterno),
            apellido_materno = COALESCE(@apellidoMaterno, apellido_materno),
            email = COALESCE(@email, email),
            contrasena = COALESCE(@contrasena, contrasena),
            rol = COALESCE(@rol::tipo_usuario, rol),
            id_carrera = COALESCE(@idCarrera, id_carrera),
            telefono = COALESCE(@telefono, telefono),
            telefono_referencia = COALESCE(@telefonoReferencia, telefono_referencia),
            nombre_referencia = COALESCE(@nombreReferencia, nombre_referencia),
            email_referencia = COALESCE(@emailReferencia, email_referencia)
            WHERE carnet = @carnet AND estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["carnet"] = comando.Carnet,
            ["nombre"] = string.IsNullOrEmpty(comando.Nombre) ? (object)DBNull.Value : comando.Nombre,
            ["apellidoPaterno"] = string.IsNullOrEmpty(comando.ApellidoPaterno) ? (object)DBNull.Value : comando.ApellidoPaterno,
            ["apellidoMaterno"] = string.IsNullOrEmpty(comando.ApellidoMaterno) ? (object)DBNull.Value : comando.ApellidoMaterno,
            ["email"] = string.IsNullOrEmpty(comando.Email) ? (object)DBNull.Value : comando.Email,
            ["contrasena"] = string.IsNullOrEmpty(comando.Contrasena) ? (object)DBNull.Value : comando.Contrasena,
            ["rol"] = string.IsNullOrEmpty(comando.Rol) ? (object)DBNull.Value : comando.Rol,
            ["idCarrera"] = idCarrera ?? (object)DBNull.Value,
            ["telefono"] = string.IsNullOrEmpty(comando.Telefono) ? (object)DBNull.Value : comando.Telefono,
            ["telefonoReferencia"] = string.IsNullOrEmpty(comando.TelefonoReferencia) ? (object)DBNull.Value : comando.TelefonoReferencia,
            ["nombreReferencia"] = string.IsNullOrEmpty(comando.NombreReferencia) ? (object)DBNull.Value : comando.NombreReferencia,
            ["emailReferencia"] = string.IsNullOrEmpty(comando.EmailReferencia) ? (object)DBNull.Value : comando.EmailReferencia
        };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        var dto = new UsuarioDto { Carnet = comando.Carnet, Nombre = comando.Nombre };
        return Result<UsuarioDto?>.Success(dto);
    }

    public Result<UsuarioDto?> Actualizar(ActualizarUsuarioComando comando)
        => Actualizar(null, comando);

    public Result<UsuarioDto?> Eliminar(EliminarUsuarioComando comando)
    {
        const string sql = @"UPDATE public.usuarios SET estado_eliminado = TRUE WHERE carnet = @carnet";
        var parametros = new Dictionary<string, object?> { ["carnet"] = comando.Carnet };
        _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        return Result<UsuarioDto?>.Success(new UsuarioDto { Carnet = comando.Carnet });
    }

    public Result<DataTable> ObtenerTodos()
    {
        const string sql = @"SELECT u.carnet, u.nombre, u.apellido_paterno, u.apellido_materno,
            c.nombre AS carrera, u.rol, u.email, u.telefono,
            u.telefono_referencia, u.nombre_referencia, u.email_referencia
            FROM public.usuarios AS u
            INNER JOIN public.carreras AS c ON u.id_carrera = c.id_carrera
            WHERE u.estado_eliminado = FALSE";
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
        return dt.Rows.Count == 0
            ? Result<DataTable>.NotFound("No se encontró el registro especificado")
            : Result<DataTable>.Success(dt);
    }

    public DataTable? ObtenerPorEmailYContrasena(string email, string contrasena)
    {
        const string sql = @"SELECT u.carnet, u.nombre, u.apellido_paterno, u.apellido_materno,
            c.nombre AS carrera, u.rol, u.email, u.telefono,
            u.telefono_referencia, u.nombre_referencia, u.email_referencia
            FROM public.usuarios AS u
            INNER JOIN public.carreras AS c ON u.id_carrera = c.id_carrera
            WHERE u.email = @email AND u.contrasena = @contrasena AND u.estado_eliminado = FALSE";
        var parametros = new Dictionary<string, object?>
        {
            ["email"] = email,
            ["contrasena"] = contrasena
        };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : dt;
    }

    public DataTable ObtenerPorCarnets(List<string> carnets)
    {
        if (carnets == null || !carnets.Any()) return new DataTable();

        const string sql = @"SELECT carnet, nombre, apellido_paterno FROM public.usuarios WHERE carnet = ANY(@carnets)";
        var parametros = new Dictionary<string, object?> { ["carnets"] = carnets };
        return _ejecutarConsulta.EjecutarFuncion(sql, parametros);
    }

    public bool ExisteActivoPorCarnet(string carnet)
    {
        const string sql = @"SELECT EXISTS(SELECT 1 FROM public.usuarios WHERE carnet = @carnet AND estado_eliminado = FALSE)";
        var parametros = new Dictionary<string, object?> { ["carnet"] = carnet };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0][0]);
    }

    public int? ObtenerCarreraIdPorNombre(string nombreCarrera)
    {
        const string sql = @"SELECT id_carrera FROM public.carreras WHERE nombre = @nombre AND estado_eliminado = FALSE LIMIT 1";
        var parametros = new Dictionary<string, object?> { ["nombre"] = nombreCarrera };
        var dt = _ejecutarConsulta.EjecutarFuncion(sql, parametros);
        return dt.Rows.Count == 0 ? null : Convert.ToInt32(dt.Rows[0][0]);
    }
}