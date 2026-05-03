using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService : Service<CategoriaEntity, CategoriaRepository, CategoriaDto>
{
    private readonly CategoriaRepository _categoriaRepository;

    public CategoriaService(CategoriaRepository repository) : base(repository)
    {
        _categoriaRepository = repository;
    }

    public override async Task<Result<CategoriaDto>> Create(CategoriaEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            return Result<CategoriaDto>.Error("Nombre de categoría es requerido");

        var existing = await _categoriaRepository.GetByNombre(entity.Nombre);
        
        if (existing != null)
            return Result<CategoriaDto>.Error($"Ya existe una categoría con nombre '{entity.Nombre}'");

        return await base.Create(entity);
    }

    public override async Task<Result<CategoriaDto>> Update(CategoriaEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            return Result<CategoriaDto>.Error("Nombre de categoría es requerido");

        var existing = await _categoriaRepository.GetByNombre(entity.Nombre);
        
        if (existing != null && existing.Id != entity.Id)
            return Result<CategoriaDto>.Error($"Ya existe otra categoría con nombre '{entity.Nombre}'");

        return await base.Update(entity);
    }
}
