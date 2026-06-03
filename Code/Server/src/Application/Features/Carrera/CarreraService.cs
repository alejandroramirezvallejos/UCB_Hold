using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;

namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService : Service<CarreraEntity, Repository<CarreraEntity, CarreraDto>, CarreraDto>
{
    private readonly AuditLogService _audit;

    public CarreraService(
        Repository<CarreraEntity, CarreraDto> repository,
        IMapper<CarreraEntity, CarreraDto> mapper,
        IValidator<CarreraDto> validator,
        AuditLogService audit)
        : base(repository, validator, mapper) => _audit = audit;

    public override async Task<Result<CarreraDto>> Create(CarreraDto dto)
    {
        var result = await base.Create(dto);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Crear, nameof(CarreraEntity), result.Value?.Id?.ToString());
        return result;
    }

    public override async Task<Result<CarreraDto>> Update(int id, CarreraDto dto)
    {
        var result = await base.Update(id, dto);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Editar, nameof(CarreraEntity), id.ToString());
        return result;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var result = await base.Delete(id);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Eliminar, nameof(CarreraEntity), id.ToString());
        return result;
    }
}
