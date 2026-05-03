using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoEntity, GrupoEquipoDto>
{
    public GrupoEquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado);

    public async Task<GrupoEquipoEntity?> GetByNombreModeloMarca(string nombre, string modelo, string marca)
        => await DbContext.GruposEquipos
            .FirstOrDefaultAsync(g => g.Nombre == nombre && g.Modelo == modelo && g.Marca == marca && !g.EstadoEliminado);

    public async Task<List<GrupoEquipoDto>> Search(string? nombre = null, string? categoria = null)
    {
        var query = DbContext.GruposEquipos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(g => g.Nombre.Contains(nombre) || g.Modelo.Contains(nombre) || g.Marca.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Join(
                DbContext.Categorias,
                g => g.IdCategoria,
                c => c.Id,
                (g, c) => new { Grupo = g, Categoria = c }
            ).Where(x => x.Categoria.Nombre == categoria).Select(x => x.Grupo);
        }

        var entities = await query.ToListAsync();
        var categoriaMap = await DbContext.Categorias.ToDictionaryAsync(c => c.Id, c => c.Nombre);

        return entities.Select(e => new GrupoEquipoDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Modelo = e.Modelo,
            Marca = e.Marca,
            Descripcion = e.Descripcion,
            UrlDataSheet = e.UrlDataSheet,
            UrlImagen = e.UrlImagen,
            IdCategoria = e.IdCategoria,
            NombreCategoria = categoriaMap.ContainsKey(e.IdCategoria) ? categoriaMap[e.IdCategoria] : null,
            Cantidad = e.Cantidad,
            CostoPromedio = e.CostoPromedio
        }).ToList();
    }

    protected override GrupoEquipoDto MapToDto(GrupoEquipoEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Modelo = entity.Modelo,
        Marca = entity.Marca,
        Descripcion = entity.Descripcion,
        UrlDataSheet = entity.UrlDataSheet,
        UrlImagen = entity.UrlImagen,
        IdCategoria = entity.IdCategoria,
        NombreCategoria = null,
        Cantidad = entity.Cantidad,
        CostoPromedio = entity.CostoPromedio
    };
}

