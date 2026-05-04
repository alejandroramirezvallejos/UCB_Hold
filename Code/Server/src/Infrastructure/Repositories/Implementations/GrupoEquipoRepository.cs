using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using IMT_Reservas.Server.Core.Common;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoEntity, GrupoEquipoDto>
{
    public GrupoEquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<GrupoEquipoDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.GruposEquipos
            .Include(g => g.Categoria)
            .AsNoTracking()
            .ToListAsync();

        return Result<List<GrupoEquipoDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<GrupoEquipoDto>> Get(int id)
    {
        var entity = await DbContext.GruposEquipos
            .Include(g => g.Categoria)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        return entity == null
            ? Result<GrupoEquipoDto>.NotFound()
            : Result<GrupoEquipoDto>.Success(MapToDto(entity));
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado);

    public async Task<GrupoEquipoEntity?> GetByNombreModeloMarca(string nombre, string modelo, string marca)
        => await DbContext.GruposEquipos
            .FirstOrDefaultAsync(g => g.Nombre == nombre && g.Modelo == modelo && g.Marca == marca && !g.EstadoEliminado);

    public async Task<List<GrupoEquipoDto>> Search(string? nombre = null, string? categoria = null)
    {
        var query = DbContext.GruposEquipos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(g => g.Nombre.Contains(nombre) || g.Modelo.Contains(nombre) || g.Marca.Contains(nombre));

        var entities = await query.ToListAsync();
        var categorias = await DbContext.Categorias.ToListAsync();

        var result = entities.Select(e =>
        {
            var cat = categorias.FirstOrDefault(c => c.Id == e.IdCategoria);
            
            return new GrupoEquipoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Marca = e.Marca,
                Descripcion = e.Descripcion,
                UrlDataSheet = e.UrlDataSheet,
                UrlImagen = e.UrlImagen,
                IdCategoria = e.IdCategoria,
                NombreCategoria = cat?.Nombre,
                Cantidad = e.Cantidad,
                CostoPromedio = e.CostoPromedio
            };
        }).ToList();

        if (!string.IsNullOrWhiteSpace(categoria))
            result = result.Where(g => g.NombreCategoria == categoria).ToList();

        return result;
    }

    protected override GrupoEquipoDto MapToDto(GrupoEquipoEntity entity)
        => new()
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Modelo = entity.Modelo,
            Marca = entity.Marca,
            Descripcion = entity.Descripcion,
            UrlDataSheet = entity.UrlDataSheet,
            UrlImagen = entity.UrlImagen,
            IdCategoria = entity.IdCategoria,
            NombreCategoria = entity.Categoria?.Nombre ?? "",
            Cantidad = entity.Cantidad,
            CostoPromedio = entity.CostoPromedio
        };
}

