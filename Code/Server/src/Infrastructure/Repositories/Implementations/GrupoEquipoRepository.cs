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
        var dtos = await DbContext.GruposEquipos
            .AsNoTracking()
            .Include(g => g.Categoria)
            .Select(e => new GrupoEquipoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Marca = e.Marca,
                Descripcion = e.Descripcion,
                UrlDataSheet = e.UrlDataSheet,
                UrlImagen = e.UrlImagen,
                IdCategoria = e.IdCategoria,
                NombreCategoria = e.Categoria.Nombre,
                Cantidad = e.Cantidad,
                CostoPromedio = e.CostoPromedio
            })
            .ToListAsync();

        return Result<List<GrupoEquipoDto>>.Success(dtos);
    }

    public override async Task<Result<GrupoEquipoDto>> Get(int id)
    {
        var dto = await DbContext.GruposEquipos
            .AsNoTracking()
            .Include(g => g.Categoria)
            .Where(g => g.Id == id && !g.EstadoEliminado)
            .Select(e => new GrupoEquipoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Marca = e.Marca,
                Descripcion = e.Descripcion,
                UrlDataSheet = e.UrlDataSheet,
                UrlImagen = e.UrlImagen,
                IdCategoria = e.IdCategoria,
                NombreCategoria = e.Categoria.Nombre,
                Cantidad = e.Cantidad,
                CostoPromedio = e.CostoPromedio
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<GrupoEquipoDto>.NotFound()
            : Result<GrupoEquipoDto>.Success(dto);
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado);

    public async Task<GrupoEquipoEntity?> GetByNombreModeloMarca(string nombre, string modelo, string marca)
        => await DbContext.GruposEquipos
            .FirstOrDefaultAsync(g => g.Nombre == nombre && g.Modelo == modelo && g.Marca == marca && !g.EstadoEliminado);

    public async Task<List<GrupoEquipoDto>> Search(string? nombre = null, string? categoria = null)
    {
        var query = DbContext.GruposEquipos
            .AsNoTracking()
            .Include(g => g.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(g => g.Nombre.Contains(nombre) || g.Modelo.Contains(nombre) || g.Marca.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(g => g.Categoria.Nombre == categoria);

        var result = await query
            .Select(e => new GrupoEquipoDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Marca = e.Marca,
                Descripcion = e.Descripcion,
                UrlDataSheet = e.UrlDataSheet,
                UrlImagen = e.UrlImagen,
                IdCategoria = e.IdCategoria,
                NombreCategoria = e.Categoria.Nombre,
                Cantidad = e.Cantidad,
                CostoPromedio = e.CostoPromedio
            })
            .ToListAsync();

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
            NombreCategoria = null,
            Cantidad = entity.Cantidad,
            CostoPromedio = entity.CostoPromedio
        };
}

