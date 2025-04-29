/*
    INFO: Segun DIP (Principio de Inversion de Dependencias)
    los modulos de alto nivel deben dependender de abstracciones 
    no de implementaciones concretas
*/
//SUGGESTION: Apliquen lo mismo a los demas Repositories
public class UsuarioRepository : IUsuarioRepository
{
    private readonly DataBaseExecuteQuery _db;

    public UsuarioRepository(DataBaseExecuteQuery db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public void Insert(Usuario usuario)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario));

        var parametros = CreateParameters(usuario);
        _db.EjecutarStoredProcedureSinRetorno("sp_insertar_usuario", parametros);
    }

    public void Update(Usuario usuario)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario));

        var parametros = CreateParameters(usuario);
        _db.EjecutarStoredProcedureSinRetorno("sp_actualizar_usuario", parametros);
    }

    public void DeleteByCarnet(string carnet)
    {
        if (string.IsNullOrWhiteSpace(carnet))
            throw new ArgumentException("El carnet no puede ser nulo o vacío.", nameof(carnet));

        var parametros = new Dictionary<string, object>
        {
            ["carnet"] = carnet
        };
        _db.EjecutarStoredProcedureSinRetorno("sp_eliminar_usuario_por_carnet", parametros);
    }

    public Usuario? GetByCarnet(string carnet)
    {
        if (string.IsNullOrWhiteSpace(carnet))
            throw new ArgumentException("El carnet no puede ser nulo o vacío.", nameof(carnet));

        var parametros = new Dictionary<string, object>
        {
            ["carnet_input"] = carnet
        };
        var tabla = _db
            .EjecutarFuncionConRetorno(
                "SELECT * FROM obtener_usuario_por_carnet(@carnet_input)",
                parametros
            )
            .GetAwaiter()
            .GetResult();

        if (tabla.Rows.Count == 0)
            return null;

        var fila = tabla.Rows[0];

        int carreraId = 0;
        if (tabla.Columns.Contains("carrera_id") && fila["carrera_id"] != DBNull.Value)
            carreraId = Convert.ToInt32(fila["carrera_id"]);

        var rolStr = fila["rol"]?.ToString() ?? string.Empty;

        return new Usuario(
            carnet:             fila["carnet"]?.ToString()              ?? string.Empty,
            nombre:             fila["nombre"]?.ToString()              ?? string.Empty,
            apellidoPaterno:    fila["apellido_paterno"]?.ToString()    ?? string.Empty,
            apellidoMaterno:    fila["apellido_materno"]?.ToString()    ?? string.Empty,
            rol:                rolStr,
            carreraId:          carreraId,
            contrasena:         fila["contrasena"]?.ToString()          ?? string.Empty,
            email:              fila["email"]?.ToString()               ?? string.Empty,
            telefono:           fila["telefono"]?.ToString()            ?? string.Empty,
            nombreReferencia:   fila["nombre_referencia"]?.ToString()   ?? string.Empty,
            telefonoReferencia: fila["telefono_referencia"]?.ToString() ?? string.Empty,
            emailReferencia:    fila["email_referencia"]?.ToString()    ?? string.Empty
        );
    }

    private Dictionary<string, object> CreateParameters(Usuario usuario) => new()
    {
        ["carnet_identidad"]    = usuario.Carnet,
        ["nombre"]              = usuario.Nombre,
        ["apellido_paterno"]    = usuario.ApellidoPaterno,
        ["apellido_materno"]    = usuario.ApellidoMaterno,
        ["tipo_usuario"]        = usuario.Rol,
        ["carrera_id"]          = usuario.CarreraId,
        ["password"]            = usuario.Contrasena,
        ["email"]               = usuario.Email,
        ["telefono"]            = usuario.Telefono,
        ["nombre_referencia"]   = usuario.NombreReferencia,
        ["telefono_referencia"] = usuario.TelefonoReferencia,
        ["email_referencia"]    = usuario.EmailReferencia
    };
}
