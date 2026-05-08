using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    private readonly GrupoEquipoRepository _grupoRepository;
    private readonly ApplicationDbContext _dbContext;

    public GrupoEquipoService(GrupoEquipoRepository repository, ApplicationDbContext dbContext) : base(repository)
    {
        _grupoRepository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<GrupoEquipoDto>> CreateFromDto(GrupoEquipoDto dto)
    {
        var idCategoria = await ResolveCategoria(dto.IdCategoria, dto.NombreCategoria);
        if (idCategoria == null)
            return Result<GrupoEquipoDto>.Error("Categoría no encontrada");

        var entity = new GrupoEquipoEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Marca = dto.Marca ?? string.Empty,
            Descripcion = dto.Descripcion ?? string.Empty,
            UrlDataSheet = dto.UrlDataSheet,
            UrlImagen = dto.UrlImagen ?? string.Empty,
            IdCategoria = idCategoria.Value,
            EstadoEliminado = false
        };

        return await Create(entity);
    }

    public async Task<Result<GrupoEquipoDto>> UpdateFromDto(int id, GrupoEquipoDto dto)
    {
        var idCategoria = await ResolveCategoria(dto.IdCategoria, dto.NombreCategoria);
        if (idCategoria == null)
            return Result<GrupoEquipoDto>.Error("Categoría no encontrada");

        var entity = new GrupoEquipoEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Marca = dto.Marca ?? string.Empty,
            Descripcion = dto.Descripcion ?? string.Empty,
            UrlDataSheet = dto.UrlDataSheet,
            UrlImagen = dto.UrlImagen ?? string.Empty,
            IdCategoria = idCategoria.Value,
            EstadoEliminado = false
        };

        return await Update(entity);
    }

    public override async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Modelo) || string.IsNullOrWhiteSpace(entity.Marca))
            return Result<GrupoEquipoDto>.Error("Nombre, modelo y marca son requeridos");

        var existing = await _grupoRepository.GetByNombreModeloMarca(entity.Nombre, entity.Modelo, entity.Marca);
        if (existing != null)
            return Result<GrupoEquipoDto>.Error($"Ya existe un grupo con nombre '{entity.Nombre}', modelo '{entity.Modelo}' y marca '{entity.Marca}'");

        return await base.Create(entity);
    }

    public override async Task<Result<GrupoEquipoDto>> Update(GrupoEquipoEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Modelo) || string.IsNullOrWhiteSpace(entity.Marca))
            return Result<GrupoEquipoDto>.Error("Nombre, modelo y marca son requeridos");

        var existing = await _grupoRepository.GetByNombreModeloMarca(entity.Nombre, entity.Modelo, entity.Marca);
        if (existing != null && existing.Id != entity.Id)
            return Result<GrupoEquipoDto>.Error($"Ya existe otro grupo con nombre '{entity.Nombre}', modelo '{entity.Modelo}' y marca '{entity.Marca}'");

        return await base.Update(entity);
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
    {
        var results = await _grupoRepository.Search(nombre, categoria);
        return Result<List<GrupoEquipoDto>>.Success(results);
    }

    private async Task<int?> ResolveCategoria(int? idCategoria, string? nombreCategoria)
    {
        if (idCategoria.HasValue && idCategoria.Value > 0)
            return idCategoria.Value;

        if (!string.IsNullOrWhiteSpace(nombreCategoria))
        {
            var cat = await _dbContext.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nombre == nombreCategoria && !c.EstadoEliminado);
            return cat?.Id;
        }

        return null;
    }
}
