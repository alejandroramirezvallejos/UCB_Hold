using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public abstract class Repository<TEntity, TDto> where TEntity : class where TDto : class
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext) => DbContext = dbContext;

    public async Task<Result<TDto>> Create(TEntity entity)
    {
        DbContext.Add(entity);
        await DbContext.SaveChangesAsync();

        return Result<TDto>.Created(MapToDto(entity));
    }

    public async Task<Result<TDto>> Update(TEntity entity)
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
        var entity = await DbContext.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => GetId(e)!.Equals(id));

        return entity == null
            ? Result<TDto>.NotFound()
            : Result<TDto>.Success(MapToDto(entity));
    }

    public virtual async Task<Result<List<TDto>>> GetAll(QueryFilter? filter = null)
    {
        var query = DbContext.Set<TEntity>().AsQueryable();
        var entities = await query.ToListAsync();
        var dtos = entities.Select(MapToDto).ToList();

        return Result<List<TDto>>.Success(dtos);
    }

    protected abstract TDto MapToDto(TEntity entity);

    protected object? GetId(TEntity entity)
    {
        var idProp = typeof(TEntity).GetProperty("Id")
                  ?? typeof(TEntity).GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));

        if (idProp == null) 
            return null;

        return idProp.GetValue(entity);
    }
}
