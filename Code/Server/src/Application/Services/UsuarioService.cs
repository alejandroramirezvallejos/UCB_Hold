public class UsuarioService : ICrearUsuarioComando, IObtenerUsuarioConsulta,
                            IActualizarUsuarioComando, IEliminarUsuarioComando,
                            IIniciarSesionUsuarioConsulta
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }
    public void Handle(CrearUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Crear(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear usuario", ex);
        }
    }
    public List<UsuarioDto>? Handle()
    {
        try
        {
            return _usuarioRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener usuarios", ex);
        }
    }
    public void Handle(ActualizarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar usuario", ex);
        }
    }
    public void Handle(EliminarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Eliminar(comando.Carnet);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar usuario", ex);
        }
    }
    public UsuarioDto? Handle(IniciarSesionUsuarioConsulta consulta)
    {
        try
        {
            return _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener usuario por email y contraseña", ex);
        }
    }
}