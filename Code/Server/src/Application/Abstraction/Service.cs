using Ardalis.Result;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
namespace IMT_Reservas.Server.Application.Abstraction;

public class Service<TEntity, TRepository, TDto>
    where TEntity : class
    where TRepository : Repository<TEntity, TDto>
    where TDto : class
{
    protected readonly TRepository Repository;

    public Service(TRepository repository) => Repository = repository;

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
}
