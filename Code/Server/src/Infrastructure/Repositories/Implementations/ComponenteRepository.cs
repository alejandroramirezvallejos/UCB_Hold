using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComponenteRepository : Repository<ComponenteEntity, ComponenteListDto>
{
    public ComponenteRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Componentes.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt)
        => await DbContext.Equipos
            .Where(e => e.CodigoImt == codigoImt && !e.EstadoEliminado)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

    protected override ComponenteListDto MapToDto(ComponenteEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Modelo = entity.Modelo,
        Descripcion = entity.Descripcion,
        Precio = (decimal?)(entity.PrecioReferencia ?? 0),
        IdEquipo = entity.IdEquipo
    };
}

