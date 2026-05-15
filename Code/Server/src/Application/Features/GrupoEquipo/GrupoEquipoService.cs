using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    private readonly GrupoEquipoRepository _grupoRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly GrupoEquipoMapper _mapper;
    private readonly IValidator<GrupoEquipoDto> _validator;

    public GrupoEquipoService(GrupoEquipoRepository repository, ApplicationDbContext dbContext, GrupoEquipoMapper mapper, IValidator<GrupoEquipoDto> validator)
        : base(repository) => (_grupoRepository, _dbContext, _mapper, _validator) = (repository, dbContext, mapper, validator);

    public async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);

        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<GrupoEquipoDto>();

        var existing = await _grupoRepository.GetByNombreModeloMarca(dto.Nombre!, dto.Modelo!, dto.Marca!);
        if (existing != null)
            return Result<GrupoEquipoDto>.Error($"Ya existe un grupo con nombre '{dto.Nombre}', modelo '{dto.Modelo}' y marca '{dto.Marca}'");

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<GrupoEquipoDto>> Update(int id, GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);

        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<GrupoEquipoDto>();

        var existing = await _grupoRepository.GetByNombreModeloMarca(dto.Nombre!, dto.Modelo!, dto.Marca!);
        if (existing != null && existing.Id != id)
            return Result<GrupoEquipoDto>.Error($"Ya existe otro grupo con nombre '{dto.Nombre}', modelo '{dto.Modelo}' y marca '{dto.Marca}'");

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        return await base.Update(entity);
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
        => Result<List<GrupoEquipoDto>>.Success(await _grupoRepository.Search(nombre, categoria));

    private async Task ResolveCategoria(GrupoEquipoDto dto)
    {
        if ((dto.IdCategoria ?? 0) > 0) return;

        if (string.IsNullOrWhiteSpace(dto.NombreCategoria)) return;

        var categoria = await _dbContext.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(categoria => categoria.Nombre == dto.NombreCategoria && !categoria.EstadoEliminado);

        dto.IdCategoria = categoria?.Id;
    }
}
