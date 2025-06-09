public class GrupoEquipoService :   ICrearGrupoEquipoComando, IObtenerGrupoEquipoConsulta,
                                    IActualizarGrupoEquipoComando,
                                    IEliminarGrupoEquipoComando, IObtenerGrupoEquipoPorIdConsulta,
                                    IObtenerGrupoEquipoPorNombreYCategoriaConsulta
{
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;

    public GrupoEquipoService(IGrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }    public void Handle(CrearGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Crear(comando);
        }
        catch
        {
            throw;
        }
    }    public GrupoEquipoDto? Handle(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        try
        {
            return _grupoEquipoRepository.ObtenerPorId(consulta.Id);
        }
        catch
        {
            throw;
        }
    }    public List<GrupoEquipoDto>? Handle()
    {
        try
        {
            return _grupoEquipoRepository.ObtenerTodos();
        }
        catch
        {
            throw;
        }
    }    public List<GrupoEquipoDto>? Handle(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta)
    {
        try
        {
            return _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
        }
        catch
        {
            throw;
        }
    }    public void Handle(ActualizarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Actualizar(comando);
        }
        catch
        {
            throw;
        }
    }    public void Handle(EliminarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Eliminar(comando.Id);
        }
        catch
        {
            throw;
        }
    }
}