using System.Data;
public class UsuarioService : BaseServicios,
    ICrearServicio<CrearUsuarioComando>,
    IActualizarServicio<ActualizarUsuarioComando>,
    IEliminarServicio<EliminarUsuarioComando>,
    IObtenerTodosServicio<UsuarioDto>
{
    private readonly UsuarioRepository _usuarioRepository;
    public UsuarioService(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
    
    public void Crear(CrearUsuarioComando comando)
    {
        ValidarEntradaCreacion(comando);

        // Resolver FK: nombre de carrera → id_carrera
        var idCarrera = _usuarioRepository.ObtenerCarreraIdPorNombre(comando.NombreCarrera!);
        if (idCarrera == null)
            throw new ErrorCarreraNoEncontrada();

        _usuarioRepository.Crear(idCarrera.Value, comando);
    }
    protected override void ValidarEntradaCreacion<T>(T comando)
    {
        base.ValidarEntradaCreacion(comando);
        if (comando is CrearUsuarioComando cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Carnet)) throw new ErrorCarnetRequerido();
            if (cmd.Carnet.Length > 15) throw new ErrorLongitudInvalida("carnet", 15);
            if (string.IsNullOrWhiteSpace(cmd.Nombre)) throw new ErrorNombreRequerido();
            if (cmd.Nombre.Length > 100) throw new ErrorLongitudInvalida("nombre", 100);
            if (string.IsNullOrWhiteSpace(cmd.ApellidoPaterno)) throw new ErrorApellidoPaternoRequerido();
            if (cmd.ApellidoPaterno.Length > 100) throw new ErrorLongitudInvalida("apellido paterno", 100);
            if (string.IsNullOrWhiteSpace(cmd.ApellidoMaterno)) throw new ErrorApellidoMaternoRequerido();
            if (cmd.ApellidoMaterno.Length > 100) throw new ErrorLongitudInvalida("apellido materno", 100);
            if (string.IsNullOrWhiteSpace(cmd.Email) || !IsValidEmail(cmd.Email)) throw new ErrorEmailInvalido();
            if (cmd.Email.Length > 150) throw new ErrorLongitudInvalida("email", 150);
            if (string.IsNullOrWhiteSpace(cmd.Contrasena)) throw new ErrorContrasenaRequerida();
            if (cmd.Contrasena.Length < 6) throw new ErrorLongitudInvalida("contraseña", 6, 100);
            if (string.IsNullOrWhiteSpace(cmd.NombreCarrera)) throw new ErrorCarreraRequerida();
            if (string.IsNullOrWhiteSpace(cmd.Telefono)) throw new ErrorTelefonoRequerido();
            if (cmd.Telefono.Length > 20) throw new ErrorLongitudInvalida("telefono", 20);
            if (cmd.Rol != "administrador" && cmd.Rol != "estudiante") throw new ErrorRolInvalido();
        }
    }

    public List<UsuarioDto>? ObtenerTodos()
    {
        try
        {
            DataTable resultado = _usuarioRepository.ObtenerTodos();
            var lista = new List<UsuarioDto>(resultado.Rows.Count);
            foreach (DataRow fila in resultado.Rows)
            {
                var dto = MapearFilaADto(fila) as UsuarioDto;
                if (dto != null) lista.Add(dto);
            }
            return lista;
        }
        catch { throw; }
    }
    public void Actualizar(ActualizarUsuarioComando comando)
    {
        ValidarEntradaActualizacion(comando);

        // Verificar que el usuario exista y esté activo
        if (!_usuarioRepository.ExisteActivoPorCarnet(comando.Carnet!))
            throw new ErrorRegistroNoEncontrado();

        // Resolver FK de carrera si se está cambiando
        int? idCarrera = null;
        if (!string.IsNullOrWhiteSpace(comando.NombreCarrera))
        {
            idCarrera = _usuarioRepository.ObtenerCarreraIdPorNombre(comando.NombreCarrera);
            if (idCarrera == null)
                throw new ErrorCarreraNoEncontrada();
        }

        // Validar rol si se proporciona
        if (!string.IsNullOrWhiteSpace(comando.Rol))
        {
            if (comando.Rol != "administrador" && comando.Rol != "estudiante")
                throw new ErrorRolInvalido();
        }

        _usuarioRepository.Actualizar(idCarrera, comando);
    }
    private void ValidarEntradaActualizacion(ActualizarUsuarioComando comando)
    {
        if (comando == null) throw new ArgumentNullException(nameof(comando));
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
    
    public void Eliminar(EliminarUsuarioComando comando)
    {
        ValidarEntradaEliminacion(comando);

        // Verificar que el usuario exista y esté activo
        if (!_usuarioRepository.ExisteActivoPorCarnet(comando.Carnet!))
            throw new ErrorRegistroNoEncontrado();

        _usuarioRepository.Eliminar(comando);
    }
    protected override void ValidarEntradaEliminacion<T>(T comando)
    {
        base.ValidarEntradaEliminacion(comando);
        if (comando is EliminarUsuarioComando cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Carnet)) throw new ErrorCarnetInvalido();
            if (cmd.Carnet.Length > 15) throw new ErrorLongitudInvalida("carnet", 15);
        }
    }

    public UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta)
    {
        try
        {
            DataTable? resultado = _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
            if (resultado?.Rows.Count > 0) return MapearFilaADto(resultado.Rows[0]) as UsuarioDto;
            return null;
        }
        catch { throw; }
    }
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
        catch { return false; }
    }
    protected override BaseDto MapearFilaADto(DataRow fila)
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