using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService : Service<MuebleEntity, MuebleRepository, MuebleDto>
{
    private readonly MuebleMapper _mapper;

    public MuebleService(MuebleRepository repository, MuebleMapper mapper, IValidator<MuebleDto> validator)
        : base(repository, validator) { _mapper = mapper; }

    protected override MuebleEntity MapToEntity(MuebleDto dto) => _mapper.ToEntity(dto);

    public Task<Result<MuebleDto>> Create(MuebleDto dto) => ValidateAndCreate(dto);

    public Task<Result<MuebleDto>> Update(int id, MuebleDto dto) => ValidateAndUpdate(id, dto);
}
