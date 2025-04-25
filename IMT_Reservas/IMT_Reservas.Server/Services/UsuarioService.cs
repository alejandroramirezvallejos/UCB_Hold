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

    public async Task<UsuarioReadDto?> ObtenerUsuarioPorCarnetAsync(string carnet)
    {
        var usuario = await _usuarioRepository.GetByCarnetAsync(carnet);
        if (usuario is null) return null;

        return new UsuarioReadDto
        {
            CarnetIdentidad = usuario.Carnet,
            Nombre = usuario.Nombre,
            ApellidoPaterno = usuario.ApellidoPaterno,
            ApellidoMaterno = usuario.ApellidoMaterno,
            Rol = usuario.Rol.ToString(),
            NombreCarrera = usuario.NombreCarrera,
            Email = usuario.Email,
            Telefono = usuario.Telefono,
            NombreReferencia = usuario.NombreReferencia,
            TelefonoReferencia = usuario.TelefonoReferencia,
            EmailReferencia = usuario.EmailReferencia
        };
    }

    public async Task CrearUsuarioAsync(Usuario usuario)
    {
        await _usuarioRepository.InsertAsync(usuario);
    }
}
