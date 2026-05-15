using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoEntity, GrupoEquipoDto>
{
    public GrupoEquipoRepository(ApplicationDbContext dbContext, GrupoEquipoMapper mapper)
        : base(dbContext, mapper) { }

    public async Task<List<GrupoEquipoDto>> Search(string? nombre = null, string? categoria = null)
    {
        var query = DbContext.GruposEquipos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(g => g.Nombre.Contains(nombre) || g.Modelo.Contains(nombre) || g.Marca.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(g => g.Categoria != null && g.Categoria.Nombre == categoria);

        return await ProjectTo(query).ToListAsync();
    }

    public async Task<int?> FindCategoriaIdByNombre(string nombre)
        => await DbContext.Categorias
            .AsNoTracking()
            .Where(c => c.Nombre == nombre && !c.EstadoEliminado)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync();
}
