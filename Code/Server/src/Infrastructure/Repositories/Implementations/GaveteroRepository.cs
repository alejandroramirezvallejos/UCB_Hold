using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GaveteroRepository : Repository<GaveteroEntity, GaveteroDto>
{
    public GaveteroRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Gaveteros.AnyAsync(g => g.Id == id && !g.EstadoEliminado);

    public async Task<bool> ExisteActivoPorNombre(string nombre)
        => await DbContext.Gaveteros.AnyAsync(g => g.Nombre == nombre && !g.EstadoEliminado);

    public async Task<bool> ExisteActivoPorNombreExcluyendoId(string nombre, int idExcluir)
        => await DbContext.Gaveteros.AnyAsync(g => g.Nombre == nombre && !g.EstadoEliminado && g.Id != idExcluir);

    public async Task<int?> GetMuebleByNombre(string nombreMueble)
        => await DbContext.Muebles
            .Where(m => m.Nombre == nombreMueble && !m.EstadoEliminado)
            .Select(m => m.Id)
            .FirstOrDefaultAsync();

    public async Task<int?> GetMuebleByGavetero(int gaveteroId)
        => await DbContext.Gaveteros
            .Where(g => g.Id == gaveteroId && !g.EstadoEliminado)
            .Select(g => g.IdMueble)
            .FirstOrDefaultAsync();

    protected override GaveteroDto MapToDto(GaveteroEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Tipo = entity.Tipo,
        NombreMueble = null,
        Longitud = entity.Longitud,
        Profundidad = entity.Profundidad,
        Altura = entity.Altura
    };
}

