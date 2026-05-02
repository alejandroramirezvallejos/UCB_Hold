using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GrupoEquipoRepository : Repository<GrupoEquipoEntity, GrupoEquipoListDto>
{
    public GrupoEquipoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.GruposEquipos.AnyAsync(g => g.Id == id && !g.EstadoEliminado);

    public async Task<GrupoEquipoEntity?> GetByNombreModeloMarca(string nombre, string modelo, string marca)
        => await DbContext.GruposEquipos
            .FirstOrDefaultAsync(g => g.Nombre == nombre && g.Modelo == modelo && g.Marca == marca && !g.EstadoEliminado);

    protected override GrupoEquipoListDto MapToDto(GrupoEquipoEntity entity) => new()
    {
        id = entity.Id,
        IdCategoria = entity.IdCategoria,
        nombre = entity.Nombre,
        modelo = entity.Modelo,
        marca = entity.Marca,
        Cantidad = entity.Cantidad,
        descripcion = entity.Descripcion,
        url_data_sheet = entity.UrlDataSheet,
        link = null,
        nombreCategoria = null,
        CostoPromedio = entity.CostoPromedio
    };
}
