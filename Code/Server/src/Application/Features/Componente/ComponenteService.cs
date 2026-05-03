using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService : Service<ComponenteEntity, ComponenteRepository, ComponenteDto>
{
    public ComponenteService(ComponenteRepository repository) : base(repository) { }
}
