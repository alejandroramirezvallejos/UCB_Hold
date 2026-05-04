using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService : Service<GaveteroEntity, GaveteroRepository, GaveteroDto>
{
    private readonly GaveteroRepository _repository;
    private readonly ApplicationDbContext _dbContext;

    public GaveteroService(GaveteroRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public override async Task<Result<GaveteroDto>> Create(GaveteroEntity entity)
    {
        var muebleExists = await _dbContext.Muebles
            .AnyAsync(m => m.Id == entity.IdMueble && !m.EstadoEliminado);
       
        if (!muebleExists)
            return Result<GaveteroDto>.Error("Mueble no existe");

        return await base.Create(entity);
    }
}
