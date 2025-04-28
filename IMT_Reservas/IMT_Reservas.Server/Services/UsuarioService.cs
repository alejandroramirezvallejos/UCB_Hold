/*  
    INFO: Extraigo IUsuarioService de UsuarioService, de modo que el 
    controlador UsuarioController dependa de una abstraccion, no de una implementacion concreta
*/
//SUGGESTION: Apliquen buenas practicas chicos
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public UsuarioReadDto? ObtenerUsuarioPorCarnet(string carnet)
    {
        if (string.IsNullOrWhiteSpace(carnet))
            throw new ArgumentException("El carnet no puede ser nulo o vacío.",
                      nameof(carnet));

        var usuario = _usuarioRepository.GetByCarnet(carnet);
        if (usuario == null)
            return null;

        return new UsuarioReadDto
        {
            CarnetIdentidad    = usuario.Carnet,
            Nombre             = usuario.Nombre,
            ApellidoPaterno    = usuario.ApellidoPaterno,
            ApellidoMaterno    = usuario.ApellidoMaterno,
            Rol                = usuario.Rol.ToString(),
            NombreCarrera      = usuario.NombreCarrera,
            Email              = usuario.Email,
            Telefono           = usuario.Telefono,
            NombreReferencia   = usuario.NombreReferencia,
            TelefonoReferencia = usuario.TelefonoReferencia,
            EmailReferencia    = usuario.EmailReferencia
        };
    }


    public void CrearUsuario(Usuario usuario)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario));

        _usuarioRepository.Insert(usuario);
    }
}

