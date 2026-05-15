using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
namespace IMT_Reservas.Server.Application.Abstraction;

public class Service<TEntity, TRepository, TDto>
    where TEntity : Entity
    where TRepository : Repository<TEntity, TDto>
    where TDto : class
{
    protected readonly TRepository Repository;
    protected readonly IValidator<TDto> Validator;
    private readonly IMapper<TEntity, TDto> _mapper;

    public Service(TRepository repository, IValidator<TDto> validator, IMapper<TEntity, TDto> mapper)
    {
        Repository = repository;
        Validator = validator;
        _mapper = mapper;
    }

    protected TEntity MapToEntity(TDto dto) => _mapper.ToEntity(dto);

    protected TDto MapToDto(TEntity entity) => _mapper.ToDto(entity);

    protected async Task<Result<TDto>> CreateEntity(TEntity entity)
        => await Repository.Create(entity);

    protected async Task<Result<TDto>> UpdateEntity(TEntity entity)
        => await Repository.Update(entity);

    public virtual Task<Result<TDto>> Create(TDto dto) => ValidateAndCreate(dto);

    public virtual Task<Result<TDto>> Update(int id, TDto dto) => ValidateAndUpdate(id, dto);

    public virtual async Task<Result<object>> Delete(int id)
        => await Repository.Delete(id);

    public virtual async Task<Result<TDto>> Get(int id)
        => await Repository.Get(id);

    public virtual async Task<Result<List<TDto>>> GetAll(QueryFilter? filter = null)
        => await Repository.GetAll(filter);

    protected async Task<Result<TDto>> ValidateAndCreate(TDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<TDto>();

        return await CreateEntity(MapToEntity(dto));
    }

    protected async Task<Result<TDto>> ValidateAndUpdate(int id, TDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<TDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;

        return await UpdateEntity(entity);
    }
}
