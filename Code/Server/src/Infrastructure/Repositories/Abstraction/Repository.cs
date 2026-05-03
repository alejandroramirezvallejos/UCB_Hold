using Ardalis.Result;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public abstract class Repository<TEntity, TDto> where TEntity : class where TDto : class
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext) => DbContext = dbContext;

    public virtual async Task<Result<TDto>> Create(TEntity entity)
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();
        
        return Result<TDto>.Created(MapToDto(entity));
    }

    public virtual async Task<Result<TDto>> Update(TEntity entity)
    {
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();
        
        return Result<TDto>.Success(MapToDto(entity));
    }

    public virtual async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.FindAsync(typeof(TEntity), id);
        
        if (entity == null)
            return Result<object>.NotFound();

        DbContext.Remove(entity);
        await DbContext.SaveChangesAsync();
        
        return Result<object>.Success(null!);
    }

    public virtual async Task<Result<TDto>> Get(int id)
    {
        var entity = await DbContext.FindAsync(typeof(TEntity), id);
        
        return entity == null
            ? Result<TDto>.NotFound()
            : Result<TDto>.Success(MapToDto((TEntity)entity));
    }

    public virtual async Task<Result<List<TDto>>> GetAll(QueryFilter? filter = null)
    {
        var query = DbContext.Set<TEntity>().AsQueryable();
        var entities = await query.ToListAsync();
        var dtos = entities.Select(MapToDto).ToList();
        
        return dtos.Count == 0
            ? Result<List<TDto>>.NotFound()
            : Result<List<TDto>>.Success(dtos);
    }

    public virtual bool Exists(int id)
    {
        return DbContext.Set<TEntity>().Any(e => GetIdValue(e).Equals(id));
    }

    protected abstract TDto MapToDto(TEntity entity);

    protected virtual int GetIdValue(TEntity entity)
    {
        var idProp = typeof(TEntity).GetProperty("Id");
        
        return idProp != null ? (int)idProp.GetValue(entity)! : 0;
    }
}
