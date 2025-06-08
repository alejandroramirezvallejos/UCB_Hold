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
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear empresa de mantenimiento", ex);
        }
    }    public List<EmpresaMantenimientoDto>? Handle()
    {
        try
        {
            return _empresaMantenimientoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener empresas de mantenimiento", ex);
        }
    }    public void Handle(ActualizarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar empresa de mantenimiento", ex);
        }
    }    public void Handle(EliminarEmpresaMantenimientoComando comando)
    {
        try
        {
            _empresaMantenimientoRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar empresa de mantenimiento", ex);
        }
    }
}