using System.Data;

/*
    INFO: Segun DIP los modulos de alto nivel deben dependender de abstracciones 
    no de implementaciones concretas
*/
//SUGGESTION: Apliquen lo mismo a los demas Repositories
public class UsuarioRepository : IUsuarioRepository
{
    private readonly DataBaseExecuteQuery _db;

    public UsuarioRepository(DataBaseExecuteQuery db)
    {
        _db = db;
    }

    public async Task InsertAsync(Usuario usuario)
    {
        var parametros = CrearParametros(usuario);
        await _db.EjecutarStoredProcedureSinRetorno("sp_insertar_usuario", parametros);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        var parametros = CrearParametros(usuario);
        await _db.EjecutarStoredProcedureSinRetorno("sp_actualizar_usuario", parametros);
    }

    public async Task DeleteByCarnetAsync(string carnet)
    {
        var parametros = new Dictionary<string, object>
        {
            { "Carnet", carnet }
        };

        await _db.EjecutarStoredProcedureSinRetorno("sp_eliminar_usuario_por_id", parametros);
    }

    public async Task<Usuario?> GetByCarnetAsync(string carnet)
    {
        var parametros = new Dictionary<string, object>
        {
            { "carnet_input", carnet }
        };

        var tabla = await _db.EjecutarFuncionConRetorno("SELECT * FROM obtener_usuario_por_carnet(@carnet_input)", parametros);

        if (tabla.Rows.Count == 0)
            return null;

        var fila = tabla.Rows[0];

        return new Usuario(
            carnet: fila["carnet"]?.ToString() ?? string.Empty,
            nombre: fila["nombre"]?.ToString() ?? string.Empty,
            apellidoPaterno: fila["apellido_paterno"]?.ToString() ?? string.Empty,
            apellidoMaterno: fila["apellido_materno"]?.ToString() ?? string.Empty,
            rol: Enum.TryParse<TipoUsuario>(fila["rol"]?.ToString(), out var rol) ? rol : TipoUsuario.Estudiante,
            carreraId: Convert.ToInt32(fila["id_carrera"]),
            contrasena: fila["contraseña"]?.ToString() ?? string.Empty,
            email: fila["email"]?.ToString() ?? string.Empty,
            telefono: fila["telefono"]?.ToString() ?? string.Empty,
            nombreReferencia: fila["nombre_referencia"]?.ToString() ?? string.Empty,
            telefonoReferencia: fila["telefono_referencia"]?.ToString() ?? string.Empty,
            emailReferencia: fila["email_referencia"]?.ToString() ?? string.Empty
        );
    }

    private Dictionary<string, object> CrearParametros(Usuario usuario)
    {
        return new Dictionary<string, object>
        {
            { "carnet_identidad", usuario.Carnet },
            { "nombre", usuario.Nombre },
            { "apellido_paterno", usuario.ApellidoPaterno },
            { "apellido_materno", usuario.ApellidoMaterno },
            { "tipo_usuario", usuario.Rol.ToString() },
            { "carrera", usuario.CarreraId },
            { "user_name", usuario.Carnet },
            { "password", usuario.Contrasena },
            { "email", usuario.Email },
            { "telefono", usuario.Telefono },
            { "nombre_referencia", usuario.NombreReferencia },
            { "telefono_referencia", usuario.TelefonoReferencia },
            { "email_referencia", usuario.EmailReferencia }
        };
    }
}
