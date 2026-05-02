using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class CategoriaRepository : Repository<CategoriaEntity, CategoriaListDto>
{
    public CategoriaRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<CategoriaEntity?> GetByNombre(string nombre)
        => await DbContext.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre && !c.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Categorias.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    protected override CategoriaListDto MapToDto(CategoriaEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre
    };
}

