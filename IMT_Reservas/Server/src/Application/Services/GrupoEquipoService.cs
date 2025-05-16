using System.Data;

public class GrupoEquipoService : ICrearGrupoEquipoComando, IObtenerGrupoEquipoConsulta,
                                  IObtenerGruposEquiposConsulta, IActualizarGrupoEquipoComando,
                                  IEliminarGrupoEquipoComando
{
    private readonly IGrupoEquipoRepository _grupoEquipoRepository;

    public GrupoEquipoService(IGrupoEquipoRepository grupoEquipoRepository)
    {
        _grupoEquipoRepository = grupoEquipoRepository;
    }

    public GrupoEquipoDto Handle(CrearGrupoEquipoComando comando)
    {
        return _grupoEquipoRepository.Crear(comando);
    }

    public GrupoEquipoDto? Handle(ObtenerGrupoEquipoConsulta consulta)
    {
        return _grupoEquipoRepository.ObtenerPorId(consulta.Id);
    }

    public List<Dictionary<string, object?>> Handle(ObtenerGruposEquiposConsulta consulta)
    {
        return _grupoEquipoRepository.ObtenerPorNombreYCategoria(consulta.Nombre, consulta.Categoria);
    }

    public GrupoEquipoDto? Handle(ActualizarGrupoEquipoComando comando)
    {
        return _grupoEquipoRepository.Actualizar(comando);
    }

    public bool Handle(EliminarGrupoEquipoComando comando)
    {
        return _grupoEquipoRepository.Eliminar(comando.Id);
    }
}