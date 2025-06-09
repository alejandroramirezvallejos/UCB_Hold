public class EmpresaMantenimientoService : ICrearEmpresaMantenimientoComando, IObtenerEmpresaMantenimientoConsulta,
                                IActualizarEmpresaMantenimientoComando,
                                IEliminarEmpresaMantenimientoComando
{
    private readonly IEmpresaMantenimientoRepository _empresaMantenimientoRepository;
    public EmpresaMantenimientoService(IEmpresaMantenimientoRepository empresaMantenimientoRepository)
    {
        _empresaMantenimientoRepository = empresaMantenimientoRepository;
    }    public void Handle(CrearEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Crear(comando);
        }
        catch
        {
            // Re-lanzar la excepción original con el mensaje detallado del repository
            throw;
        }
    }    public List<EmpresaMantenimientoDto>? Handle()
    {
        try
        {
            return _empresaMantenimientoRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}