using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService : Service<MuebleEntity, MuebleRepository, MuebleDto>
{
    private readonly MuebleMapper _mapper;
    private readonly IValidator<MuebleDto> _validator;

    public MuebleService(MuebleRepository repository, MuebleMapper mapper, IValidator<MuebleDto> validator)
        : base(repository) => (_mapper, _validator) = (mapper, validator);

    public async Task<Result<MuebleDto>> Create(MuebleDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<MuebleDto>();

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<MuebleDto>> Update(int id, MuebleDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<MuebleDto>();

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        
        return await base.Update(entity);
    }
}
