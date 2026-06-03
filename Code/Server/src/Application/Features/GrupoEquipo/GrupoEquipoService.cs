using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Cache;
using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    private static readonly TimeSpan GrupoEquipoSearchTtl = TimeSpan.FromMinutes(5);
    private readonly CacheRepository _cacheRepository;
    private readonly AuditLogService _audit;

    public GrupoEquipoService(GrupoEquipoRepository repository,
        GrupoEquipoMapper mapper, IValidator<GrupoEquipoDto> validator,
        CacheRepository cacheRepository, AuditLogService audit)
        : base(repository, validator, mapper)
    {
        _cacheRepository = cacheRepository;
        _audit = audit;
    }

    public override async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<GrupoEquipoDto>();

        var createResult = await CreateEntity(MapToEntity(dto));

        if (createResult.IsSuccess)
        {
            _ = await _cacheRepository.Remove(CacheKeys.GrupoEquipoSearch(string.Empty, null));
            await _audit.Log(AuditAccion.Crear, nameof(GrupoEquipoEntity), createResult.Value?.Id?.ToString());
        }

        return createResult;
    }

    public override async Task<Result<GrupoEquipoDto>> Update(int id, GrupoEquipoDto dto)
    {
        dto.Id = id;
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<GrupoEquipoDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;

        var updateResult = await UpdateEntity(entity);

        if (updateResult.IsSuccess)
        {
            _ = await _cacheRepository.Remove(CacheKeys.GrupoEquipoSearch(string.Empty, null));
            await _audit.Log(AuditAccion.Editar, nameof(GrupoEquipoEntity), id.ToString());
        }

        return updateResult;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var deleteResult = await base.Delete(id);

        if (deleteResult.IsSuccess)
        {
            _ = await _cacheRepository.Remove(CacheKeys.GrupoEquipoSearch(string.Empty, null));
            await _audit.Log(AuditAccion.Eliminar, nameof(GrupoEquipoEntity), id.ToString());
        }

        return deleteResult;
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
    {
        var cacheKey = CacheKeys.GrupoEquipoSearch(nombre ?? string.Empty, categoria);
        var cacheResult = await _cacheRepository.Get<List<GrupoEquipoDto>>(cacheKey);
        
        if (cacheResult.IsSuccess) 
            return Result<List<GrupoEquipoDto>>.Success(cacheResult.Value);

        var equipos = await Repository.Search(nombre, categoria);
        _ = await _cacheRepository.Set(cacheKey, equipos, GrupoEquipoSearchTtl);
        
        return Result<List<GrupoEquipoDto>>.Success(equipos);
    }

    private async Task ResolveCategoria(GrupoEquipoDto dto)
    {
        if ((dto.IdCategoria ?? 0) > 0)
            return;

        if (string.IsNullOrWhiteSpace(dto.NombreCategoria))
            return;

        dto.IdCategoria = await Repository.FindCategoriaIdByNombre(dto.NombreCategoria);
    }
}
