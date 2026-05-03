using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService : Service<MuebleEntity, MuebleRepository, MuebleDto>
{
    public MuebleService(MuebleRepository repository) : base(repository) { }
}
