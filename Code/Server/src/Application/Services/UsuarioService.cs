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
        catch (ErrorCarnetInvalido)
        {
            throw;
        }
        catch (ErrorEmailInvalido)
        {
            throw;
        }
        catch (ErrorCampoRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
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
    }    private void ValidarEntradaCreacion(CrearUsuarioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Carnet))
            throw new ErrorCarnetInvalido();

        if (comando.Carnet.Length > 15)
            throw new ErrorLongitudInvalida("carnet", 15);

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 100)
            throw new ErrorLongitudInvalida("nombre", 100);

        if (string.IsNullOrWhiteSpace(comando.ApellidoPaterno))
            throw new ErrorCampoRequerido("apellido paterno");

        if (comando.ApellidoPaterno.Length > 100)
            throw new ErrorLongitudInvalida("apellido paterno", 100);

        if (string.IsNullOrWhiteSpace(comando.ApellidoMaterno))
            throw new ErrorCampoRequerido("apellido materno");

        if (comando.ApellidoMaterno.Length > 100)
            throw new ErrorLongitudInvalida("apellido materno", 100);

        if (string.IsNullOrWhiteSpace(comando.Email) || !IsValidEmail(comando.Email))
            throw new ErrorEmailInvalido();

        if (comando.Email.Length > 150)
            throw new ErrorLongitudInvalida("email", 150);

        if (string.IsNullOrWhiteSpace(comando.Contrasena))
            throw new ErrorCampoRequerido("contraseña");

        if (comando.Contrasena.Length < 6)
            throw new ErrorLongitudInvalida("contraseña", 6, 100);

        if (string.IsNullOrWhiteSpace(comando.NombreCarrera))
            throw new ErrorCampoRequerido("carrera");

        if (string.IsNullOrWhiteSpace(comando.Telefono))
            throw new ErrorCampoRequerido("telefono");

        if (comando.Telefono.Length > 20)
            throw new ErrorLongitudInvalida("telefono", 20);
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
    }    public void ActualizarUsuario(ActualizarUsuarioComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _usuarioRepository.Actualizar(comando);
        }
        catch (ErrorCarnetInvalido)
        {
            throw;
        }
        catch (ErrorNombreRequerido)
        {
            throw;
        }
        catch (ErrorEmailInvalido)
        {
            throw;
        }
        catch (ErrorCampoRequerido)
        {
            throw;
        }
        catch (ErrorLongitudInvalida)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["carnet"] = comando.Carnet,
                ["email"] = comando.Email,
                ["nombre"] = comando.Nombre
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "actualizar", "usuario", parametros);
        }
    }

    public void EliminarUsuario(EliminarUsuarioComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _usuarioRepository.Eliminar(comando.Carnet);
        }
        catch (ErrorCarnetInvalido)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parametros = new Dictionary<string, object?>
            {
                ["carnet"] = comando.Carnet
            };
            throw PostgreSqlErrorInterpreter.InterpretarError(ex, "eliminar", "usuario", parametros);
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

    private void ValidarEntradaActualizacion(ActualizarUsuarioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Carnet))
            throw new ErrorCarnetInvalido();

        if (comando.Carnet.Length > 15)
            throw new ErrorLongitudInvalida("carnet", 15);

        if (string.IsNullOrWhiteSpace(comando.Nombre))
            throw new ErrorNombreRequerido();

        if (comando.Nombre.Length > 100)
            throw new ErrorLongitudInvalida("nombre", 100);

        if (string.IsNullOrWhiteSpace(comando.ApellidoPaterno))
            throw new ErrorCampoRequerido("apellido paterno");

        if (comando.ApellidoPaterno.Length > 100)
            throw new ErrorLongitudInvalida("apellido paterno", 100);

        if (string.IsNullOrWhiteSpace(comando.ApellidoMaterno))
            throw new ErrorCampoRequerido("apellido materno");

        if (comando.ApellidoMaterno.Length > 100)
            throw new ErrorLongitudInvalida("apellido materno", 100);

        if (string.IsNullOrWhiteSpace(comando.Email) || !IsValidEmail(comando.Email))
            throw new ErrorEmailInvalido();

        if (comando.Email.Length > 150)
            throw new ErrorLongitudInvalida("email", 150);

        if (!string.IsNullOrWhiteSpace(comando.Telefono) && comando.Telefono.Length > 20)
            throw new ErrorLongitudInvalida("telefono", 20);
    }

    private void ValidarEntradaEliminacion(EliminarUsuarioComando comando)
    {
        if (comando == null)
            throw new ArgumentNullException(nameof(comando));

        if (string.IsNullOrWhiteSpace(comando.Carnet))
            throw new ErrorCarnetInvalido();

        if (comando.Carnet.Length > 15)
            throw new ErrorLongitudInvalida("carnet", 15);
    }
}