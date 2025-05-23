using System.Data;

public class EquipoService : IObtenerEquipoConsulta, ICrearEquipoComando, 
                             IActualizarEquipoComando, IEliminarEquipoComando
{
    private readonly IEquipoRepository _equipoRepository;

    public EquipoService(IEquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }

    public EquipoDto Handle(CrearEquipoComando comando)
    {
        return _equipoRepository.Crear(comando);
    }

    public EquipoDto? Handle(ActualizarEquipoComando comando)
    {
        return _equipoRepository.Actualizar(comando);
    }

    public bool Handle(EliminarEquipoComando comando)
    {
        return _equipoRepository.Eliminar(comando.Id);
    }

    public EquipoDto? Handle(ObtenerEquipoConsulta consulta)
    {
        return _equipoRepository.ObtenerPorId(consulta.Id);
    }
}