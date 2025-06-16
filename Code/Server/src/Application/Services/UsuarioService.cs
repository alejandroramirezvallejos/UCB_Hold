using System.Data;
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
            if (comando == null)
                throw new ArgumentNullException(nameof(comando), "Los datos del usuario son requeridos");

            if (string.IsNullOrWhiteSpace(comando.Carnet))
                throw new ArgumentException("El carnet es obligatorio", nameof(comando.Carnet));

            if (string.IsNullOrWhiteSpace(comando.Nombre))
                throw new ArgumentException("El nombre es obligatorio", nameof(comando.Nombre));

            if (string.IsNullOrWhiteSpace(comando.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio", nameof(comando.ApellidoPaterno));

            if (string.IsNullOrWhiteSpace(comando.ApellidoMaterno))
                throw new ArgumentException("El apellido materno es obligatorio", nameof(comando.ApellidoMaterno));

            if (string.IsNullOrWhiteSpace(comando.Email) || !IsValidEmail(comando.Email))
                throw new ArgumentException("El email es obligatorio y debe ser válido", nameof(comando.Email));            if (string.IsNullOrWhiteSpace(comando.Contrasena))
                throw new ArgumentException("La contraseña es obligatoria", nameof(comando.Contrasena));

            if (string.IsNullOrWhiteSpace(comando.NombreCarrera))
                throw new ArgumentException("El nombre de la carrera es obligatorio", nameof(comando.NombreCarrera));

            if (string.IsNullOrWhiteSpace(comando.Telefono))
                throw new ArgumentException("El teléfono es obligatorio", nameof(comando.Telefono));

            _usuarioRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
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