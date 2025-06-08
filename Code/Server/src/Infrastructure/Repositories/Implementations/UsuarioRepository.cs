//implementar
using System.Data;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IExecuteQuery _ejecutarConsulta;
    public UsuarioRepository(IExecuteQuery ejecutarConsulta)
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
        };

        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al crear el usuario", ex);
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
        };

        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, parametros);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el usuario", ex);
        }
    }

    public void Eliminar(int carnet)
    {
        const string sql = @"
        CALL public.eliminar_usuario(
	    @carnet
        )";
        try
        {
            _ejecutarConsulta.EjecutarSpNR(sql, new Dictionary<string, object?>
            {
                ["carnet"] = carnet
            });
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar el usuario", ex);
        }
    }

    public List<UsuarioDto> ObtenerTodos()
    {
        const string sql = @"
        SELECT * from public.obtener_usuarios()";
        try
        {
            var resultado = _ejecutarConsulta.EjecutarFuncion(sql, new Dictionary<string, object?>());
            List<UsuarioDto> usuarios = new List<UsuarioDto>();
            foreach (DataRow fila in resultado.Rows)
            {
                usuarios.Add(MapearUsuarioADto(fila));
            }
            return usuarios;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los usuarios", ex);
        }
    }

    private UsuarioDto MapearUsuarioADto(DataRow fila)
    {
        return new UsuarioDto
        {
            Carnet = fila["carnet"] == DBNull.Value ? null : fila["carnet"].ToString(),
            Nombre = fila["nombre"] == DBNull.Value ? null : fila["nombre"].ToString(),
            ApellidoPaterno = fila["apellido_paterno"] == DBNull.Value ? null : fila["apellido_paterno"].ToString(),
            ApellidoMaterno = fila["apellido_materno"] == DBNull.Value ? null : fila["apellido_materno"].ToString(),
            CarreraNombre = fila["carrera"] == DBNull.Value ? null : fila["carrera"].ToString(),
            Rol = fila["rol"] == DBNull.Value ? null : fila["rol"].ToString(),
            Email = fila["email"] == DBNull.Value ? null : fila["email"].ToString(),
            Telefono = fila["telefono"] == DBNull.Value ? null : fila["telefono"].ToString(),
            TelefonoReferencia = fila["telefono_referencia"] == DBNull.Value ? null : fila["telefono_referencia"].ToString(),
            NombreReferencia = fila["nombre_referencia"] == DBNull.Value ? null : fila["nombre_referencia"].ToString(),
            EmailReferencia = fila["email_referencia"] == DBNull.Value ? null : fila["email_referencia"].ToString()
        };
    }
}