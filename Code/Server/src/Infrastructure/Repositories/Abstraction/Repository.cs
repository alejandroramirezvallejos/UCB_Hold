using System.Linq.Expressions;
using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public class Repository<TEntity, TDto> where TEntity : class where TDto : class
{
    protected readonly ApplicationDbContext DbContext;
    private readonly IMapper<TEntity, TDto> _mapper;

    protected Repository(ApplicationDbContext dbContext, IMapper<TEntity, TDto> mapper)
    {
        DbContext = dbContext;
        _mapper = mapper;
    }

    protected TDto MapToDto(TEntity entity) => _mapper.ToDto(entity);

    protected IQueryable<TDto> ProjectTo(IQueryable<TEntity> source) => _mapper.ProjectTo(source);

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
        var param = Expression.Parameter(typeof(TEntity), "e");
        var predicate = Expression.Lambda<Func<TEntity, bool>>(
            Expression.Equal(
                Expression.Property(param, "Id"),
                Expression.Constant(id)),
            param);

        var dto = await _mapper.ProjectTo(
                DbContext.Set<TEntity>().AsNoTracking().Where(predicate))
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<TDto>.NotFound()
            : Result<TDto>.Success(dto);
    }

    public virtual async Task<Result<List<TDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await _mapper.ProjectTo(DbContext.Set<TEntity>().AsNoTracking()).ToListAsync();
        return Result<List<TDto>>.Success(dtos);
    }
}
