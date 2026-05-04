using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Core.Common;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService
{
    private readonly GrupoEquipoRepository _grupoRepository;

    public GrupoEquipoService(GrupoEquipoRepository repository)
    {
        _grupoRepository = repository;
    }

    public async Task<Result<List<GrupoEquipoDto>>> GetAll(QueryFilter? filter = null) => await _grupoRepository.GetAll(filter);
    
    public async Task<Result<GrupoEquipoDto>> Get(int id) => await _grupoRepository.Get(id);

    public async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Modelo) || string.IsNullOrWhiteSpace(entity.Marca))
            return Result<GrupoEquipoDto>.Error("Nombre, modelo y marca son requeridos");

        var existing = await _grupoRepository.GetByNombreModeloMarca(entity.Nombre, entity.Modelo, entity.Marca);

        if (existing != null)
            return Result<GrupoEquipoDto>.Error($"Ya existe un grupo con nombre '{entity.Nombre}', modelo '{entity.Modelo}' y marca '{entity.Marca}'");

        return await _grupoRepository.Create(entity);
    }

    public async Task<Result<GrupoEquipoDto>> Update(GrupoEquipoEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Modelo) || string.IsNullOrWhiteSpace(entity.Marca))
            return Result<GrupoEquipoDto>.Error("Nombre, modelo y marca son requeridos");

        var existing = await _grupoRepository.GetByNombreModeloMarca(entity.Nombre, entity.Modelo, entity.Marca);

        if (existing != null && existing.Id != entity.Id)
            return Result<GrupoEquipoDto>.Error($"Ya existe otro grupo con nombre '{entity.Nombre}', modelo '{entity.Modelo}' y marca '{entity.Marca}'");

        return await _grupoRepository.Update(entity);
    }

    public async Task<Result<object>> Delete(int id) => await _grupoRepository.Delete(id);

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
    {
        var results = await _grupoRepository.Search(nombre, categoria);

        return Result<List<GrupoEquipoDto>>.Success(results);
    }
}
