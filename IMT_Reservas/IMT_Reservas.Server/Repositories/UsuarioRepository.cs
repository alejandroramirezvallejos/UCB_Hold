
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class UsuarioRepository
{
    private readonly DataBaseExecuteQuery _db;

    public UsuarioRepository(DataBaseExecuteQuery db)
    {
        _db = db;
    }
    public async Task insertarUsuario(UsuarioCreateDto entity)
    {
        var parametros = new Dictionary<string, object>
        {
            { "carnet_identidad", entity.carnet_identidad },
            { "nombre", entity.nombre },
            { "apellido_paterno", entity.apellido_paterno },
            { "apellido_materno", entity.apellido_materno },
            { "tipo_usuario", entity.tipo_usuario },
            { "carrera", entity.carrera },
            { "user_name", entity.user_name },
            { "password", entity.password },
            { "email", entity.email },
            { "telefono", entity.telefono },
            { "nombre_referencia", entity.nombre_referencia },
            { "telefono_referencia", entity.telefono_referencia },
            { "email_referencia", entity.email_referencia }
        };

        await _db.EjecutarStoredProcedureSinRetorno("sp_insertar_usuario", parametros);
    }

    public async Task eliminarUsuarioPorId(int id)
    {
        var parametros = new Dictionary<string, object>
        {
            { "Carnet", id }
        };

        await _db.EjecutarStoredProcedureSinRetorno("sp_eliminar_usuario_por_id", parametros);
    }

    public async Task<Usuario> obtenerUsuarioPorCarnet(string Carnet)
    {
        var parametros = new Dictionary<string, object>
        {
            { "carnet_input", Carnet }
        };

        var tabla=await _db.EjecutarFuncionConRetorno("SELECT * FROM obtener_usuario_por_carnet(@carnet_input)", parametros);
        if(tabla.Rows.Count == 0)
        {
            return null;
        }
        var fila = tabla.Rows[0];
        return new Usuario
        {
            Carnet = fila["carnet"] != DBNull.Value ? fila["carnet"].ToString() : null,
            Nombre = fila["nombre"] != DBNull.Value ? fila["nombre"].ToString() : null,
            Apellido_Paterno = fila["apellido_paterno"] != DBNull.Value ? fila["apellido_paterno"].ToString() : null,
            Apellido_Materno = fila["apellido_materno"] != DBNull.Value ? fila["apellido_materno"].ToString() : null,
            Rol = fila["rol"] != DBNull.Value ? fila["rol"].ToString() : null,
            Id_Carrera = Convert.ToInt32(fila["id_carrera"]),
            Contraseña = fila["contraseña"].ToString(),
            Email = fila["email"] != DBNull.Value ? fila["email"].ToString() : null,
            Telefono = fila["telefono"] != DBNull.Value ? fila["telefono"].ToString() : null,
            Telefono_Referencia = fila["telefono_referencia"] != DBNull.Value ? fila["telefono_referencia"].ToString() : null,
            Nombre_Referencia = fila["nombre_referencia"] != DBNull.Value ? fila["nombre_referencia"].ToString() : null,
            Email_Referencia = fila["email_referencia"] != DBNull.Value ? fila["email_referencia"].ToString() : null
        };
    }

    public async Task actualizarUsuario(UsuarioCreateDto entidad)
    {
        var parametros = new Dictionary<string, object>
        {
            { "carnet_identidad", entidad.carnet_identidad },
            { "nombre", entidad.nombre },
            { "apellido_paterno", entidad.apellido_paterno },
            { "apellido_materno", entidad.apellido_materno },
            { "tipo_usuario", entidad.tipo_usuario },
            { "carrera", entidad.carrera },
            { "user_name", entidad.user_name },
            { "password", entidad.password },
            { "email", entidad.email },
            { "telefono", entidad.telefono },
            { "nombre_referencia", entidad.nombre_referencia },
            { "telefono_referencia", entidad.telefono_referencia },
            { "email_referencia", entidad.email_referencia }
        };

        await _db.EjecutarStoredProcedureSinRetorno("sp_actualizar_usuario", parametros);
    }
}