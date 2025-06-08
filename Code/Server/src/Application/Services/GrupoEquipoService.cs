using System.Data;

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
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al crear grupo de equipo", ex);
        }
    }    public GrupoEquipoDto? Handle(ObtenerGrupoEquipoPorIdConsulta consulta)
    {
        try
        {
            return _grupoEquipoRepository.ObtenerPorId(consulta.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener grupo de equipo por ID", ex);
        }
    }    public List<GrupoEquipoDto>? Handle()
    {
        try
        {
            return _grupoEquipoRepository.ObtenerTodos();
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener grupos de equipo", ex);
        }
    }    public List<GrupoEquipoDto>? Handle(ObtenerGrupoEquipoPorNombreYCategoriaConsulta consulta)
    {
        try
        {
            return _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al obtener grupos de equipo por nombre y categoría", ex);
        }
    }    public void Handle(ActualizarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Actualizar(comando);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al actualizar grupo de equipo", ex);
        }
    }    public void Handle(EliminarGrupoEquipoComando comando)
    {
        try
        {
            _grupoEquipoRepository.Eliminar(comando.Id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error en el servicio al eliminar grupo de equipo", ex);
        }
    }
}