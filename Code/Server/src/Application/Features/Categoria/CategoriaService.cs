using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService : Service<CategoriaEntity, CategoriaRepository, CategoriaDto>
{
    private readonly CategoriaRepository _categoriaRepository;
    private readonly CategoriaMapper _mapper;
    private readonly IValidator<CategoriaDto> _validator;

    public CategoriaService(CategoriaRepository repository, CategoriaMapper mapper, IValidator<CategoriaDto> validator)
        : base(repository) => (_categoriaRepository, _mapper, _validator) = (repository, mapper, validator);

    public async Task<Result<CategoriaDto>> Create(CategoriaDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<CategoriaDto>();

        var existing = await _categoriaRepository.GetByNombre(dto.Nombre!);
        if (existing != null)
            return Result<CategoriaDto>.Error($"Ya existe una categoría con nombre '{dto.Nombre}'");

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<CategoriaDto>> Update(int id, CategoriaDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<CategoriaDto>();

        var existing = await _categoriaRepository.GetByNombre(dto.Nombre!);
        if (existing != null && existing.Id != id)
            return Result<CategoriaDto>.Error($"Ya existe otra categoría con nombre '{dto.Nombre}'");

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        return await base.Update(entity);
    }
}
