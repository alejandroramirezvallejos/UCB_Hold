using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService : Service<EquipoEntity, EquipoRepository, EquipoDto>
{
    public EquipoService(EquipoRepository repository, EquipoMapper mapper, IValidator<EquipoDto> validator)
        : base(repository, validator, mapper) { }

    public override async Task<Result<EquipoDto>> Create(EquipoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<EquipoDto>();

        var entity = MapToEntity(dto);
        entity.CodigoImt = await Repository.GetMaxCodigoImt() + 1;
        entity.FechaIngresoEquipo = DateOnly.FromDateTime(DateTime.Now);

        var result = await CreateEntity(entity);

        if (result.IsSuccess)
            await Repository.RecalcGrupoStats(entity.IdGrupoEquipo);

        return result;
    }

    public override async Task<Result<EquipoDto>> Update(int id, EquipoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<EquipoDto>();

        var existing = await Repository.FindById(id);

        if (existing == null)
            return Result<EquipoDto>.NotFound();

        var entity = MapToEntity(dto);
        entity.Id = id;
        entity.CodigoImt = existing.CodigoImt;
        entity.FechaIngresoEquipo = existing.FechaIngresoEquipo;
        entity.EstadoEliminado = existing.EstadoEliminado;

        var result = await UpdateEntity(entity);

        if (!result.IsSuccess)
            return result;

        await Repository.RecalcGrupoStats(entity.IdGrupoEquipo);

        if (existing.IdGrupoEquipo != entity.IdGrupoEquipo)
            await Repository.RecalcGrupoStats(existing.IdGrupoEquipo);

        return result;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var existing = await Repository.FindById(id);

        if (existing == null)
            return Result<object>.NotFound();

        var result = await base.Delete(id);

        if (result.IsSuccess)
            await Repository.RecalcGrupoStats(existing.IdGrupoEquipo);

        return result;
    }
}
