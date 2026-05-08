using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Core.Abstraction;
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
        var dtos = await DbContext.Categorias
            .AsNoTracking()
            .Select(e => new CategoriaDto { Id = e.Id, Nombre = e.Nombre })
            .ToListAsync();

        return Result<List<CategoriaDto>>.Success(dtos);
    }

    public override async Task<Result<CategoriaDto>> Get(int id)
    {
        var dto = await DbContext.Categorias
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(e => new CategoriaDto { Id = e.Id, Nombre = e.Nombre })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<CategoriaDto>.NotFound()
            : Result<CategoriaDto>.Success(dto);
    }

    public async Task<CategoriaEntity?> GetByNombre(string nombre)
        => await DbContext.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Nombre == nombre && !c.EstadoEliminado);

    protected override CategoriaDto MapToDto(CategoriaEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre
    };
}


