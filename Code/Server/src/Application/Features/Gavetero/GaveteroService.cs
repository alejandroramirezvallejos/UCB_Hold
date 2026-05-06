using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService : Service<GaveteroEntity, GaveteroRepository, GaveteroDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GaveteroRepository _repository;

    public GaveteroService(GaveteroRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _dbContext = dbContext;
        _repository = repository;
    }

    public override async Task<Result<GaveteroDto>> Create(GaveteroEntity entity)
    {
        var muebleId = entity.IdMueble;
        if (muebleId <= 0)
            return Result<GaveteroDto>.Error("Mueble no existe");

        var muebleExists = await _dbContext.Muebles
            .AnyAsync(m => m.Id == muebleId && !m.EstadoEliminado);

        if (!muebleExists)
            return Result<GaveteroDto>.Error("Mueble no existe");

        return await base.Create(entity);
    }

    public override async Task<Result<GaveteroDto>> Update(GaveteroEntity entity)
    {
        var muebleId = entity.IdMueble;
        if (muebleId <= 0)
            return Result<GaveteroDto>.Error("Mueble no existe");

        var muebleExists = await _dbContext.Muebles
            .AnyAsync(m => m.Id == muebleId && !m.EstadoEliminado);

        if (!muebleExists)
            return Result<GaveteroDto>.Error("Mueble no existe");

        return await base.Update(entity);
    }

    public async Task<int?> ResolveMuebleId(string? nombreMueble, int? gaveteroId = null)
    {
        if (!string.IsNullOrWhiteSpace(nombreMueble))
            return await _repository.GetMuebleByNombre(nombreMueble);

        if (gaveteroId.HasValue && gaveteroId.Value > 0)
            return await _repository.GetMuebleByGavetero(gaveteroId.Value);

        return null;
    }
}
