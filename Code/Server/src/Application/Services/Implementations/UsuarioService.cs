using System.Data;
public class UsuarioService : BaseServicios, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }    public void CrearUsuario(CrearUsuarioComando comando)
    {
        try
        {
            ValidarEntradaCreacion(comando);
            _usuarioRepository.Crear(comando);
        }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorCarnetInvalido) { throw; }
        catch (ErrorEmailInvalido) { throw; }
        catch (ErrorCampoRequerido) { throw; }
        catch (ErrorCarnetRequerido) { throw; }
        catch (ErrorApellidoPaternoRequerido) { throw; }
        catch (ErrorApellidoMaternoRequerido) { throw; }
        catch (ErrorContrasenaRequerida) { throw; }
        catch (ErrorCarreraRequerida) { throw; }
        catch (ErrorTelefonoRequerido) { throw; }
        catch (ErrorRolInvalido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorCreacion(comando, ex);
            throw;
        }
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
    protected override void InterpretarErrorCreacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorCreacion(comando, ex);
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró la carrera con nombre")) throw new ErrorCarreraNoEncontrada();
            if (errorDb.SqlState == "23505" || mensaje.Contains("el carnet o email ya está registrado")) throw new ErrorRegistroYaExiste();
            if (mensaje.Contains("hubo un error inesperado durante el proceso de inserción")) throw new Exception($"Error inesperado al insertar usuario: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al crear usuario: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al crear usuario: {errorRepo.Message}", errorRepo);
    }    public List<UsuarioDto>? ObtenerTodosUsuarios()
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
    public void ActualizarUsuario(ActualizarUsuarioComando comando)
    {
        try
        {
            ValidarEntradaActualizacion(comando);
            _usuarioRepository.Actualizar(comando);
        }
        catch (ErrorCarnetInvalido) { throw; }
        catch (ErrorNombreRequerido) { throw; }
        catch (ErrorEmailInvalido) { throw; }
        catch (ErrorCampoRequerido) { throw; }
        catch (ErrorLongitudInvalida) { throw; }
        catch (ErrorRolInvalido) { throw; }
        catch (Exception ex)
        {
            if (ex is ErrorDataBase errorDb)
            {
                var mensaje = errorDb.Message?.ToLower() ?? "";
                if (mensaje.Contains("no existe usuario activo con carnet")) throw new ErrorRegistroNoEncontrado();
                if (mensaje.Contains("carrera no encontrada o eliminada")) throw new ErrorCarreraNoEncontrada();
                if (mensaje.Contains("rol inválido") && mensaje.Contains("debe ser administrador o estudiante")) throw new ErrorRolInvalido();
                if (errorDb.SqlState == "23505" || mensaje.Contains("ya está en uso") || mensaje.Contains("violación de unicidad")) throw new ErrorRegistroYaExiste();
                if (mensaje.Contains("error inesperado al actualizar usuario")) throw new Exception($"Error inesperado al actualizar usuario: {errorDb.Message}", errorDb);
                throw new Exception($"Error inesperado de base de datos al actualizar usuario: {errorDb.Message}", errorDb);
            }
            if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al actualizar usuario: {errorRepo.Message}", errorRepo);
            throw;
        }
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
    }    public void EliminarUsuario(EliminarUsuarioComando comando)
    {
        try
        {
            ValidarEntradaEliminacion(comando);
            _usuarioRepository.Eliminar(comando.Carnet);
        }
        catch (ErrorCarnetInvalido) { throw; }
        catch (Exception ex)
        {
            InterpretarErrorEliminacion(comando, ex);
            throw;
        }
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
    protected override void InterpretarErrorEliminacion<T>(T comando, Exception ex)
    {
        base.InterpretarErrorEliminacion(comando, ex);
        if (ex is ErrorDataBase errorDb)
        {
            var mensaje = errorDb.Message?.ToLower() ?? "";
            if (mensaje.Contains("no se encontró un usuario activo con carnet")) throw new ErrorRegistroNoEncontrado();
            if (mensaje.Contains("error al eliminar lógicamente el usuario")) throw new Exception($"Error inesperado al eliminar usuario: {errorDb.Message}", errorDb);
            throw new Exception($"Error inesperado de base de datos al eliminar usuario: {errorDb.Message}", errorDb);
        }
        if (ex is ErrorRepository errorRepo) throw new Exception($"Error del repositorio al eliminar usuario: {errorRepo.Message}", errorRepo);
    }    public UsuarioDto? IniciarSesionUsuario(IniciarSesionUsuarioConsulta consulta)
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