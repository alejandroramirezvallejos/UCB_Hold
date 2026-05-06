using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService : Service<ComponenteEntity, ComponenteRepository, ComponenteDto>
{
    private readonly ComponenteRepository _repository;

    public ComponenteService(ComponenteRepository repository)
        : base(repository)
    {
        _repository = repository;
    }

    public override async Task<Result<ComponenteDto>> Create(ComponenteEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<ComponenteDto>.Error("Equipo no encontrado");

        return await base.Create(entity);
    }

    public override async Task<Result<ComponenteDto>> Update(ComponenteEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<ComponenteDto>.Error("Equipo no encontrado");

        return await base.Update(entity);
    }

    public async Task<int?> ResolveEquipoId(int? equipoId, string? codigoImt)
    {
        if (equipoId.HasValue && equipoId.Value > 0)
            return equipoId;

        if (!string.IsNullOrWhiteSpace(codigoImt) && int.TryParse(codigoImt, out var codigoImtInt))
            return await _repository.GetEquipoByCodigoImt(codigoImtInt);

        return null;
    }
}
