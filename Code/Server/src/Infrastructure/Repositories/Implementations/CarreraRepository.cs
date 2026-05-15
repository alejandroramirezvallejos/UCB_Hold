using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarreraRepository : Repository<CarreraEntity, CarreraDto>
{
    public CarreraRepository(ApplicationDbContext dbContext, CarreraMapper mapper)
        : base(dbContext, mapper) { }
}
