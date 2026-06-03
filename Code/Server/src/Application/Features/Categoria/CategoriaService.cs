using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;

namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService : Service<CategoriaEntity, Repository<CategoriaEntity, CategoriaDto>, CategoriaDto>
{
    private readonly AuditLogService _audit;

    public CategoriaService(
        Repository<CategoriaEntity, CategoriaDto> repository,
        IMapper<CategoriaEntity, CategoriaDto> mapper,
        IValidator<CategoriaDto> validator,
        AuditLogService audit)
        : base(repository, validator, mapper) => _audit = audit;

    public override async Task<Result<CategoriaDto>> Create(CategoriaDto dto)
    {
        var result = await base.Create(dto);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Crear, nameof(CategoriaEntity), result.Value?.Id?.ToString());
        return result;
    }

    public override async Task<Result<CategoriaDto>> Update(int id, CategoriaDto dto)
    {
        var result = await base.Update(id, dto);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Editar, nameof(CategoriaEntity), id.ToString());
        return result;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var result = await base.Delete(id);
        if (result.IsSuccess)
            await _audit.Log(AuditAccion.Eliminar, nameof(CategoriaEntity), id.ToString());
        return result;
    }
}
