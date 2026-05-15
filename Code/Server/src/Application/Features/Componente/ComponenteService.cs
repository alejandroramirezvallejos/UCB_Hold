using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService : Service<ComponenteEntity, ComponenteRepository, ComponenteDto>
{
    private readonly ComponenteMapper _mapper;

    public ComponenteService(ComponenteRepository repository, ComponenteMapper mapper,
        IValidator<ComponenteDto> validator)
        : base(repository, validator)
    {
        _mapper = mapper;
    }

    protected override ComponenteEntity MapToEntity(ComponenteDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<ComponenteDto>> Create(ComponenteDto dto)
    {
        await ResolveEquipoId(dto);
        
        return await ValidateAndCreate(dto);
    }

    public async Task<Result<ComponenteDto>> Update(int id, ComponenteDto dto)
    {
        await ResolveEquipoId(dto);
        
        return await ValidateAndUpdate(id, dto);
    }

    private async Task ResolveEquipoId(ComponenteDto dto)
    {
        if ((dto.IdEquipo ?? 0) > 0) return;

        if (!string.IsNullOrWhiteSpace(dto.CodigoImtEquipo)
            && int.TryParse(dto.CodigoImtEquipo, out var codigoImtInt))
            dto.IdEquipo = await Repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;
    }
}
