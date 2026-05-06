using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService : Service<AccesorioEntity, AccesorioRepository, AccesorioDto>
{
    private readonly AccesorioRepository _repository;

    public AccesorioService(AccesorioRepository repository)
        : base(repository)
    {
        _repository = repository;
    }

    public override async Task<Result<AccesorioDto>> Create(AccesorioEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<AccesorioDto>.Error("Equipo no encontrado");

        return await base.Create(entity);
    }

    public override async Task<Result<AccesorioDto>> Update(AccesorioEntity entity)
    {
        if (entity.IdEquipo <= 0)
            return Result<AccesorioDto>.Error("Equipo no encontrado");

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
