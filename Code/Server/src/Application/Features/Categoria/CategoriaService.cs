using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService : Service<CategoriaEntity, CategoriaRepository, CategoriaDto>
{
    private readonly CategoriaMapper _mapper;

    public CategoriaService(CategoriaRepository repository, CategoriaMapper mapper, IValidator<CategoriaDto> validator)
        : base(repository, validator) { _mapper = mapper; }

    protected override CategoriaEntity MapToEntity(CategoriaDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<CategoriaDto>> Create(CategoriaDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<CategoriaDto>();

        return await base.Create(MapToEntity(dto));
    }

    public async Task<Result<CategoriaDto>> Update(int id, CategoriaDto dto)
    {
        dto.Id = id;
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<CategoriaDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;
        
        return await base.Update(entity);
    }
}
