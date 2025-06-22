using System.Data;
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioService(IUsuarioRepository usuarioRepository) => _usuarioRepository = usuarioRepository;

    public void CrearUsuario(CrearUsuarioComando comando)
    {
        ValidarEntradaCreacion(comando);
        _usuarioRepository.Crear(comando);
    }
    public List<UsuarioDto>? ObtenerTodosUsuarios()
    {
        DataTable resultado = _usuarioRepository.ObtenerTodos();
        var lista = new List<UsuarioDto>(resultado.Rows.Count);
        foreach (DataRow fila in resultado.Rows)
            lista.Add(MapearFilaADto(fila));
        return lista;
    }
    public void ActualizarUsuario(ActualizarUsuarioComando comando)
    {
        ValidarEntradaActualizacion(comando);
        _usuarioRepository.Actualizar(comando);
    }
    public void EliminarUsuario(EliminarUsuarioComando comando)
    {
        ValidarEntradaEliminacion(comando);
        _usuarioRepository.Eliminar(comando.Carnet);
    }
    public UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta)
    {
        DataTable? resultado = _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
        if (resultado?.Rows.Count > 0)
            return MapearFilaADto(resultado.Rows[0]);
        return null;
    }
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
        catch { return false; }
    }
    private void ValidarEntradaCreacion(CrearUsuarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Carnet)) throw new ErrorCarnetRequerido();
        if (comando.Carnet.Length > 15) throw new ErrorLongitudInvalida("carnet", 15);
        if (string.IsNullOrWhiteSpace(comando.Nombre)) throw new ErrorNombreRequerido();
        if (comando.Nombre.Length > 100) throw new ErrorLongitudInvalida("nombre", 100);
        if (string.IsNullOrWhiteSpace(comando.ApellidoPaterno)) throw new ErrorApellidoPaternoRequerido();
        if (comando.ApellidoPaterno.Length > 100) throw new ErrorLongitudInvalida("apellido paterno", 100);
        if (string.IsNullOrWhiteSpace(comando.ApellidoMaterno)) throw new ErrorApellidoMaternoRequerido();
        if (comando.ApellidoMaterno.Length > 100) throw new ErrorLongitudInvalida("apellido materno", 100);
        if (string.IsNullOrWhiteSpace(comando.Email) || !IsValidEmail(comando.Email)) throw new ErrorEmailInvalido();
        if (comando.Email.Length > 150) throw new ErrorLongitudInvalida("email", 150);
        if (string.IsNullOrWhiteSpace(comando.Contrasena)) throw new ErrorContrasenaRequerida();
        if (comando.Contrasena.Length < 6) throw new ErrorLongitudInvalida("contraseña", 6, 100);
        if (string.IsNullOrWhiteSpace(comando.NombreCarrera)) throw new ErrorCarreraRequerida();
        if (string.IsNullOrWhiteSpace(comando.Telefono)) throw new ErrorTelefonoRequerido();
        if (comando.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
        if(comando.Rol != "administrador" && comando.Rol != "estudiante") throw new ErrorRolInvalido();
    }
    private void ValidarEntradaActualizacion(ActualizarUsuarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (!string.IsNullOrWhiteSpace(comando.Carnet) && comando.Carnet.Length > 15) throw new ErrorLongitudInvalida("carnet", 15);
        if (!string.IsNullOrWhiteSpace(comando.Nombre) && comando.Nombre.Length > 255) throw new ErrorLongitudInvalida("nombre usuario", 255);
        if (!string.IsNullOrWhiteSpace(comando.ApellidoPaterno) && comando.ApellidoPaterno.Length > 255) throw new ErrorLongitudInvalida("apellido paterno", 255);
        if (!string.IsNullOrWhiteSpace(comando.ApellidoMaterno) && comando.ApellidoMaterno.Length > 255) throw new ErrorLongitudInvalida("apellido materno", 255);
        if (!string.IsNullOrWhiteSpace(comando.Email) && comando.Email.Length > 150) throw new ErrorLongitudInvalida("email", 150);
        if (!string.IsNullOrWhiteSpace(comando.Email) && !IsValidEmail(comando.Email)) throw new ErrorEmailInvalido();
        if (!string.IsNullOrWhiteSpace(comando.Contrasena) && comando.Contrasena.Length < 6) throw new ErrorLongitudInvalida("contraseña", 6, 100);
        if (!string.IsNullOrWhiteSpace(comando.Telefono) && comando.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
        if(comando.Rol != "administrador" && comando.Rol != "estudiante") throw new ErrorRolInvalido();
    }
    private void ValidarEntradaEliminacion(EliminarUsuarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException();
        if (string.IsNullOrWhiteSpace(comando.Carnet)) throw new ErrorCarnetInvalido();
        if (comando.Carnet.Length > 15) throw new ErrorLongitudInvalida("carnet", 15);
    }
    private UsuarioDto MapearFilaADto(DataRow fila) => new UsuarioDto
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