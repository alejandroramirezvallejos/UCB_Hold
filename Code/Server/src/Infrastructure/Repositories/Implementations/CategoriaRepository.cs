using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CategoriaRepository : Repository<CategoriaEntity, CategoriaDto>
{
    public CategoriaRepository(ApplicationDbContext dbContext, CategoriaMapper mapper)
        : base(dbContext, mapper) { }
}
