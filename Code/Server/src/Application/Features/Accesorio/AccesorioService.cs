using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService : Service<AccesorioEntity, AccesorioRepository, AccesorioDto>
{
    public AccesorioService(AccesorioRepository repository, AccesorioMapper mapper, IValidator<AccesorioDto> validator)
        : base(repository, validator, mapper) { }

    public override async Task<Result<AccesorioDto>> Create(AccesorioDto dto)
    {
        await ResolveEquipoId(dto);
        
        return await ValidateAndCreate(dto);
    }

    public override async Task<Result<AccesorioDto>> Update(int id, AccesorioDto dto)
    {
        await ResolveEquipoId(dto);
        
        return await ValidateAndUpdate(id, dto);
    }

    private async Task ResolveEquipoId(AccesorioDto dto)
    {
        if ((dto.IdEquipo ?? 0) > 0) return;

        if (!string.IsNullOrWhiteSpace(dto.CodigoImtEquipoAsociado)
            && int.TryParse(dto.CodigoImtEquipoAsociado, out var codigoImtInt))
            dto.IdEquipo = await Repository.GetEquipoByCodigoImt(codigoImtInt) ?? 0;
    }
}
