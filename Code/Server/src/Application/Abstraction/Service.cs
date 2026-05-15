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

    protected Service(TRepository repository, IValidator<TDto> validator)
    {
        Repository = repository;
        Validator = validator;
    }

    protected virtual TEntity MapToEntity(TDto dto) =>
        throw new NotSupportedException($"{GetType().Name} must override MapToEntity.");

    public virtual async Task<Result<TDto>> Create(TEntity entity)
        => await Repository.Create(entity);

    public virtual async Task<Result<TDto>> Update(TEntity entity)
        => await Repository.Update(entity);

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

        return await Create(MapToEntity(dto));
    }

    protected async Task<Result<TDto>> ValidateAndUpdate(int id, TDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<TDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;
        
        return await Update(entity);
    }
}
