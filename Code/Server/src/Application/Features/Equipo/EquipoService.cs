using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService : Service<EquipoEntity, EquipoRepository, EquipoDto>
{
    public EquipoService(EquipoRepository repository) : base(repository) { }
}
