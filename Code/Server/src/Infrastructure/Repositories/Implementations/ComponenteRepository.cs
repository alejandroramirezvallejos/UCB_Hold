using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Core.Abstraction;
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
        var dtos = await DbContext.Componentes
            .AsNoTracking()
            .Select(e => new ComponenteDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Tipo = e.Tipo,
                Descripcion = e.Descripcion,
                PrecioReferencia = e.PrecioReferencia,
                IdEquipo = e.IdEquipo,
                NombreEquipo = null,
                CodigoImtEquipo = null,
                UrlDataSheet = e.UrlDataSheet
            })
            .ToListAsync();

        return Result<List<ComponenteDto>>.Success(dtos);
    }

    public override async Task<Result<ComponenteDto>> Get(int id)
    {
        var dto = await DbContext.Componentes
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(e => new ComponenteDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Modelo = e.Modelo,
                Tipo = e.Tipo,
                Descripcion = e.Descripcion,
                PrecioReferencia = e.PrecioReferencia,
                IdEquipo = e.IdEquipo,
                NombreEquipo = null,
                CodigoImtEquipo = null,
                UrlDataSheet = e.UrlDataSheet
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<ComponenteDto>.NotFound()
            : Result<ComponenteDto>.Success(dto);
    }
    
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

