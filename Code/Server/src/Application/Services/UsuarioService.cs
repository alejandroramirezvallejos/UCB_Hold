public class UsuarioService : ICrearUsuarioComando, IObtenerUsuarioConsulta,
                            IActualizarUsuarioComando, IEliminarUsuarioComando,
                            IIniciarSesionUsuarioConsulta
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }    public void Handle(CrearUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public List<UsuarioDto>? Handle()
    {
        try
        {
            return _usuarioRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarUsuarioComando comando)
    {
        try
        {
            _usuarioRepository.Eliminar(comando.Carnet);
        }
        catch
        {
            throw;
        }
    }    public UsuarioDto? Handle(IniciarSesionUsuarioConsulta consulta)
    {
        try
        {
            return _usuarioRepository.ObtenerPorEmailYContrasena(consulta.Email, consulta.Contrasena);
        }
        catch
        {
            throw;
        }
    }
}