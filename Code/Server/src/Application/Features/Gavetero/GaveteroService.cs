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

    public async Task<Result<GaveteroDto>> CreateFromDto(GaveteroDto dto)
    {
        var muebleId = 0;
        if (!string.IsNullOrWhiteSpace(dto.NombreMueble))
            muebleId = await _repository.GetMuebleByNombre(dto.NombreMueble) ?? 0;

        var entity = new GaveteroEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Tipo = dto.Tipo,
            IdMueble = muebleId,
            Longitud = dto.Longitud,
            Profundidad = dto.Profundidad,
            Altura = dto.Altura
        };
        return await Create(entity);
    }

    public async Task<Result<GaveteroDto>> UpdateFromDto(int id, GaveteroDto dto)
    {
        var muebleId = 0;
        if (!string.IsNullOrWhiteSpace(dto.NombreMueble))
            muebleId = await _repository.GetMuebleByNombre(dto.NombreMueble) ?? 0;
        else
            muebleId = await _repository.GetMuebleByGavetero(id) ?? 0;

        var entity = new GaveteroEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Tipo = dto.Tipo,
            IdMueble = muebleId,
            Longitud = dto.Longitud,
            Profundidad = dto.Profundidad,
            Altura = dto.Altura
        };
        return await Update(entity);
    }
}
