using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComponenteRepository : Repository<ComponenteEntity, ComponenteDto>
{
    public ComponenteRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<ComponenteDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.Componentes
            .AsNoTracking()
            .ToListAsync();

        return Result<List<ComponenteDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<ComponenteDto>> Get(int id)
    {
        var entity = await DbContext.Componentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        return entity == null
            ? Result<ComponenteDto>.NotFound()
            : Result<ComponenteDto>.Success(MapToDto(entity));
    }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Componentes.AnyAsync(c => c.Id == id && !c.EstadoEliminado);

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt)
        => await DbContext.Equipos
            .Where(e => e.CodigoImt == codigoImt && !e.EstadoEliminado)
            .Select(e => e.Id)
            .FirstOrDefaultAsync();

    protected override ComponenteDto MapToDto(ComponenteEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Modelo = entity.Modelo,
        Tipo = entity.Tipo,
        Descripcion = entity.Descripcion,
        PrecioReferencia = entity.PrecioReferencia,
        IdEquipo = entity.IdEquipo,
        NombreEquipo = null,
        CodigoImtEquipo = null,
        UrlDataSheet = entity.UrlDataSheet
    };
}

