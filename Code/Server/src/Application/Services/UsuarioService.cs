using System.Data;
using Shared.Common;
public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _usuarioRepository;

    public UsuarioService(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }    public void CrearUsuario(CrearUsuarioComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _usuarioRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorCarnetUsuarioNoEncontrado)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["carnet"] = comando.Carnet,
                ["email"] = comando.Email,
                ["nombre"] = comando.Nombre,
                ["carrera"] = comando.NombreCarrera
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "crear", "usuario", parametros);
        }
    }

    private void ValidarEntradaCreacion(CrearUsuarioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Carnet))
            throw new ErrorNombreRequerido("carnet del usuario");

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido("nombre del usuario");

        if (string.IsNullOrWhiteSpace(comando.ApellidoPaterno))
            throw new ErrorNombreRequerido("apellido paterno");

        if (string.IsNullOrWhiteSpace(comando.ApellidoMaterno))
            throw new ErrorNombreRequerido("apellido materno");

        if (string.IsNullOrWhiteSpace(comando.Email) || !IsValidEmail(comando.Email))
            throw new ErrorNombreRequerido("email válido");

        if (string.IsNullOrWhiteSpace(comando.Contrasena))
            throw new ErrorNombreRequerido("contraseña");

        if (string.IsNullOrWhiteSpace(comando.NombreCarrera))
            throw new ErrorNombreRequerido("nombre de la carrera");

        if (string.IsNullOrWhiteSpace(comando.Telefono))
            throw new ErrorNombreRequerido("teléfono");
    }
    public List<UsuarioDto>? ObtenerTodosUsuarios()
    {
        try
        {
            DataTable resultado = _usuarioRepository.ObtenerTodos();
            var lista = new List<UsuarioDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                lista.Add(MapearFilaADto(fila));
            }
            return lista;
        }
        catch
        {
            throw;
        }
    }
    public void ActualizarUsuario(ActualizarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }
    public void EliminarUsuario(EliminarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Eliminar(comando.Carnet);
        }
        catch
        {
            throw;
        }
    }
    public UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta)
    {
        try
        {
            DataTable? resultado = _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
            if (resultado?.Rows.Count > 0)
            {
                return MapearFilaADto(resultado.Rows[0]);
            }
            return null;
        }
        catch
        {
            throw;
        }
    }
    private UsuarioDto MapearFilaADto(DataRow fila)
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
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}