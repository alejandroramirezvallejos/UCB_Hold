using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService : Service<AccesorioEntity, AccesorioRepository, AccesorioDto>
{
    private readonly AccesorioRepository _repository;
    private readonly AccesorioMapper _mapper;
    private readonly IValidator<AccesorioDto> _validator;

    public AccesorioService(AccesorioRepository repository, AccesorioMapper mapper, IValidator<AccesorioDto> validator)
        : base(repository) => (_repository, _mapper, _validator) = (repository, mapper, validator);

    public async Task<Result<AccesorioDto>> Create(AccesorioDto dto)
    {
        await ResolveEquipoId(dto);

        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<AccesorioDto>();

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<AccesorioDto>> Update(int id, AccesorioDto dto)
    {
        await ResolveEquipoId(dto);

        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<AccesorioDto>();

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        return await base.Update(entity);
    }

    private async Task ResolveEquipoId(AccesorioDto dto)
    {
        if ((dto.IdEquipo ?? 0) > 0) return;

        if (!string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado)
            && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImtInt))
            dto.IdEquipo = await _repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;
    }
}
