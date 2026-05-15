using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoEntity, GrupoEquipoDto>
{
    private readonly GrupoEquipoMapper _mapper;

    public GrupoEquipoRepository(ApplicationDbContext dbContext, GrupoEquipoMapper mapper)
        : base(dbContext) => _mapper = mapper;

    public override async Task<Result<List<GrupoEquipoDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.GruposEquipos
            .AsNoTracking()
            .Select(grupoEquipo => new GrupoEquipoDto
            {
                Id = grupoEquipo.Id,
                Nombre = grupoEquipo.Nombre,
                Modelo = grupoEquipo.Modelo,
                Marca = grupoEquipo.Marca,
                Descripcion = grupoEquipo.Descripcion,
                UrlDataSheet = grupoEquipo.UrlDataSheet,
                UrlImagen = grupoEquipo.UrlImagen,
                IdCategoria = grupoEquipo.IdCategoria,
                NombreCategoria = grupoEquipo.Categoria != null ? grupoEquipo.Categoria.Nombre : string.Empty,
                Cantidad = grupoEquipo.Cantidad,
                CostoPromedio = grupoEquipo.CostoPromedio
            })
            .ToListAsync();

        return Result<List<GrupoEquipoDto>>.Success(dtos);
    }

    public override async Task<Result<GrupoEquipoDto>> Get(int id)
    {
        var dto = await DbContext.GruposEquipos
            .AsNoTracking()
            .Where(grupoEquipo => grupoEquipo.Id == id && !grupoEquipo.EstadoEliminado)
            .Select(grupoEquipo => new GrupoEquipoDto
            {
                Id = grupoEquipo.Id,
                Nombre = grupoEquipo.Nombre,
                Modelo = grupoEquipo.Modelo,
                Marca = grupoEquipo.Marca,
                Descripcion = grupoEquipo.Descripcion,
                UrlDataSheet = grupoEquipo.UrlDataSheet,
                UrlImagen = grupoEquipo.UrlImagen,
                IdCategoria = grupoEquipo.IdCategoria,
                NombreCategoria = grupoEquipo.Categoria != null ? grupoEquipo.Categoria.Nombre : string.Empty,
                Cantidad = grupoEquipo.Cantidad,
                CostoPromedio = grupoEquipo.CostoPromedio
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<GrupoEquipoDto>.NotFound()
            : Result<GrupoEquipoDto>.Success(dto);
    }

    public async Task<GrupoEquipoEntity?> GetByNombreModeloMarca(string nombre, string modelo, string marca)
        => await DbContext.GruposEquipos
            .FirstOrDefaultAsync(grupoEquipo => grupoEquipo.Nombre == nombre && grupoEquipo.Modelo == modelo && grupoEquipo.Marca == marca && !grupoEquipo.EstadoEliminado);

    public async Task<List<GrupoEquipoDto>> Search(string? nombre = null, string? categoria = null)
    {
        var query = DbContext.GruposEquipos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(grupoEquipo => grupoEquipo.Nombre.Contains(nombre) || grupoEquipo.Modelo.Contains(nombre) || grupoEquipo.Marca.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(grupoEquipo => grupoEquipo.Categoria != null && grupoEquipo.Categoria.Nombre == categoria);

        return await query
            .Select(grupoEquipo => new GrupoEquipoDto
            {
                Id = grupoEquipo.Id,
                Nombre = grupoEquipo.Nombre,
                Modelo = grupoEquipo.Modelo,
                Marca = grupoEquipo.Marca,
                Descripcion = grupoEquipo.Descripcion,
                UrlDataSheet = grupoEquipo.UrlDataSheet,
                UrlImagen = grupoEquipo.UrlImagen,
                IdCategoria = grupoEquipo.IdCategoria,
                NombreCategoria = grupoEquipo.Categoria != null ? grupoEquipo.Categoria.Nombre : string.Empty,
                Cantidad = grupoEquipo.Cantidad,
                CostoPromedio = grupoEquipo.CostoPromedio
            })
            .ToListAsync();
    }

    protected override GrupoEquipoDto MapToDto(GrupoEquipoEntity entity) => _mapper.ToDto(entity);
}
