using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CarreraRepository : Repository<CarreraEntity, CarreraDto>
{
    public CarreraRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<CarreraDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Carreras
            .AsNoTracking()
            .Select(e => new CarreraDto { Id = e.Id, Nombre = e.Nombre })
            .ToListAsync();

        return Result<List<CarreraDto>>.Success(dtos);
    }

    public override async Task<Result<CarreraDto>> Get(int id)
    {
        var entity = await DbContext.Carreras
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity == null
            ? Result<CarreraDto>.NotFound()
            : Result<CarreraDto>.Success(MapToDto(entity));
    }

    public async Task<CarreraEntity?> GetByNombre(string nombre)
        => await DbContext.Carreras.FirstOrDefaultAsync(c => c.Nombre == nombre && !c.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Carreras.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    protected override CarreraDto MapToDto(CarreraEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre
    };
}


