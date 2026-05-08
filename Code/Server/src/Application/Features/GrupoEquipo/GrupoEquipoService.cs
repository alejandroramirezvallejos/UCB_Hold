using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Common;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    private readonly GrupoEquipoRepository _grupoRepository;

    public GrupoEquipoService(GrupoEquipoRepository repository) : base(repository)
    {
        _grupoRepository = repository;
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
}
