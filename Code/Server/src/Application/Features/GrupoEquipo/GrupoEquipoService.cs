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
    private static readonly TimeSpan VersionTtl = TimeSpan.FromDays(30);
    private readonly CacheRepository _cacheRepository;

    public GrupoEquipoService(GrupoEquipoRepository repository,
        GrupoEquipoMapper mapper, IValidator<GrupoEquipoDto> validator,
        CacheRepository cacheRepository, AuditLogService audit)
        : base(repository, validator, mapper, audit) => 
        _cacheRepository = cacheRepository;

    public override async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid) 
            return validation.ToResult<GrupoEquipoDto>();

        var createResult = await CreateEntity(MapToEntity(dto));

        if (createResult.IsSuccess)
        {
            await BumpSearchVersion();
            await Audit!.Log(AuditAccion.Crear, typeof(GrupoEquipoEntity).Name, createResult.Value?.Id?.ToString());
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
            await BumpSearchVersion();
            await Audit!.Log(AuditAccion.Editar, typeof(GrupoEquipoEntity).Name, id.ToString());
        }

        return updateResult;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var deleteResult = await base.Delete(id);

        if (deleteResult.IsSuccess)
            await BumpSearchVersion();
        
        return deleteResult;
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
    {
        var version = await GetSearchVersion();
        var cacheKey = CacheKeys.GrupoEquipoSearch(nombre ?? string.Empty, categoria, version);
        var cacheResult = await _cacheRepository.Get<List<GrupoEquipoDto>>(cacheKey);

        if (cacheResult.IsSuccess)
            return Result<List<GrupoEquipoDto>>.Success(cacheResult.Value);

        var equipos = await Repository.Search(nombre, categoria);
        _ = await _cacheRepository.Set(cacheKey, equipos, GrupoEquipoSearchTtl);

        return Result<List<GrupoEquipoDto>>.Success(equipos);
    }

    private async Task<long> GetSearchVersion()
    {
        var result = await _cacheRepository.Get<long>(CacheKeys.GrupoEquipoVersion);
        return result.IsSuccess ? result.Value : 0;
    }

    private async Task BumpSearchVersion()
    {
        var next = await GetSearchVersion() + 1;
        _ = await _cacheRepository.Set(CacheKeys.GrupoEquipoVersion, next, VersionTtl);
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
