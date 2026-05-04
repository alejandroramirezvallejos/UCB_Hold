using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CategoriaRepository : Repository<CategoriaEntity, CategoriaDto>
{
    public CategoriaRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<CategoriaDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.Categorias
            .AsNoTracking()
            .ToListAsync();

        return Result<List<CategoriaDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<CategoriaDto>> Get(int id)
    {
        var entity = await DbContext.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity == null
            ? Result<CategoriaDto>.NotFound()
            : Result<CategoriaDto>.Success(MapToDto(entity));
    }

    public async Task<CategoriaEntity?> GetByNombre(string nombre)
        => await DbContext.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre && !c.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Categorias.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    protected override CategoriaDto MapToDto(CategoriaEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre
    };
}


