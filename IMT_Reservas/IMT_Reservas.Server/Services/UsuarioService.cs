public class UsuarioService
{
    private readonly UsuarioRepository _usuarioRepository;

    public UsuarioService(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioReadDto>? ObtenerUsuarioPorCarnet(string carnet)
    {
        var usuario= await _usuarioRepository.obtenerUsuarioPorCarnet(carnet);
        return new UsuarioReadDto
        {
            Carnet = usuario.Carnet,
            Nombre = usuario.Nombre,
            Apellido_Paterno = usuario.Apellido_Paterno,
            Apellido_Materno = usuario.Apellido_Materno,
            Rol = usuario.Rol,
            Id_Carrera = usuario.Id_Carrera,
            Email = usuario.Email,
            Telefono = usuario.Telefono,
            Telefono_Referencia = usuario.Telefono_Referencia,
            Nombre_Referencia = usuario.Nombre_Referencia,
            Email_Referencia = usuario.Email_Referencia
        };
    }

    public void CrearUsuario(Usuario usuario)
    {
    }
}