using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService : Service<GaveteroEntity, GaveteroRepository, GaveteroDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GaveteroRepository _repository;
    private readonly GaveteroMapper _mapper;
    private readonly IValidator<GaveteroDto> _validator;

    public GaveteroService(GaveteroRepository repository, ApplicationDbContext dbContext, GaveteroMapper mapper, IValidator<GaveteroDto> validator)
        : base(repository) => (_repository, _dbContext, _mapper, _validator) = (repository, dbContext, mapper, validator);

    public async Task<Result<GaveteroDto>> Create(GaveteroDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<GaveteroDto>();

        var muebleId = await _repository.GetMuebleByNombre(dto.NombreMueble!) ?? 0;
        if (muebleId <= 0) return Result<GaveteroDto>.Error("Mueble no existe");

        var entity = _mapper.ToEntity(dto);
        entity.IdMueble = muebleId;

        var result = await base.Create(entity);
        if (result.IsSuccess) await RecalcMuebleCount(muebleId);
        return result;
    }

    public async Task<Result<GaveteroDto>> Update(int id, GaveteroDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<GaveteroDto>();

        var muebleId = !string.IsNullOrWhiteSpace(dto.NombreMueble)
            ? await _repository.GetMuebleByNombre(dto.NombreMueble) ?? 0
            : await _repository.GetMuebleByGavetero(id) ?? 0;

        if (muebleId <= 0) return Result<GaveteroDto>.Error("Mueble no existe");

        var previousMuebleId = await _repository.GetMuebleByGavetero(id);
        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        entity.IdMueble = muebleId;

        var result = await base.Update(entity);
        if (!result.IsSuccess) return result;

        await RecalcMuebleCount(muebleId);
        if (previousMuebleId.HasValue && previousMuebleId.Value != muebleId)
            await RecalcMuebleCount(previousMuebleId.Value);
        return result;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var muebleId = await _repository.GetMuebleByGavetero(id);
        var result = await base.Delete(id);
        if (result.IsSuccess && muebleId.HasValue) await RecalcMuebleCount(muebleId.Value);
        return result;
    }

    private async Task RecalcMuebleCount(int muebleId)
    {
        var mueble = await _dbContext.Muebles.FirstOrDefaultAsync(mueble => mueble.Id == muebleId);
        if (mueble == null) return;

        mueble.NumeroGaveteros = await _dbContext.Gaveteros
            .CountAsync(gavetero => gavetero.IdMueble == muebleId && !gavetero.EstadoEliminado);

        await _dbContext.SaveChangesAsync();
    }
}
