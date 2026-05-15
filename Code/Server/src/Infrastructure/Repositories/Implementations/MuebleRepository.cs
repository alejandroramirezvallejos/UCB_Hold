using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleEntity, MuebleDto>
{
    public MuebleRepository(ApplicationDbContext dbContext, MuebleMapper mapper)
        : base(dbContext, mapper) { }
}
