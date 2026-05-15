using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ComponenteRepository : Repository<ComponenteEntity, ComponenteDto>
{
    private readonly ComponenteMapper _mapper;

    public ComponenteRepository(ApplicationDbContext dbContext, ComponenteMapper mapper)
        : base(dbContext) => _mapper = mapper;

    public override async Task<Result<List<ComponenteDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await (
            from componente in DbContext.Componentes.AsNoTracking()
            join equipo in DbContext.Equipos.AsNoTracking()
                on componente.IdEquipo equals equipo.Id into equipoJoin
            from equipo in equipoJoin.DefaultIfEmpty()
            select new ComponenteDto
            {
                Id = componente.Id,
                Nombre = componente.Nombre,
                Modelo = componente.Modelo,
                Tipo = componente.Tipo,
                Descripcion = componente.Descripcion,
                PrecioReferencia = componente.PrecioReferencia,
                IdEquipo = componente.IdEquipo,
                NombreEquipo = equipo != null ? equipo.Descripcion : null,
                CodigoImtEquipo = equipo != null ? equipo.CodigoImt.ToString() : null,
                UrlDataSheet = componente.UrlDataSheet
            }
        ).ToListAsync();

        return Result<List<ComponenteDto>>.Success(dtos);
    }

    public override async Task<Result<ComponenteDto>> Get(int id)
    {
        var dto = await (
            from componente in DbContext.Componentes.AsNoTracking().Where(componente => componente.Id == id)
            join equipo in DbContext.Equipos.AsNoTracking()
                on componente.IdEquipo equals equipo.Id into equipoJoin
            from equipo in equipoJoin.DefaultIfEmpty()
            select new ComponenteDto
            {
                Id = componente.Id,
                Nombre = componente.Nombre,
                Modelo = componente.Modelo,
                Tipo = componente.Tipo,
                Descripcion = componente.Descripcion,
                PrecioReferencia = componente.PrecioReferencia,
                IdEquipo = componente.IdEquipo,
                NombreEquipo = equipo != null ? equipo.Descripcion : null,
                CodigoImtEquipo = equipo != null ? equipo.CodigoImt.ToString() : null,
                UrlDataSheet = componente.UrlDataSheet
            }
        ).FirstOrDefaultAsync();

        return dto == null
            ? Result<ComponenteDto>.NotFound()
            : Result<ComponenteDto>.Success(dto);
    }

    public async Task<int?> GetEquipoByCodigoImt(int codigoImt)
        => await DbContext.Equipos
            .Where(equipo => equipo.CodigoImt == codigoImt && !equipo.EstadoEliminado)
            .Select(equipo => equipo.Id)
            .FirstOrDefaultAsync();

    protected override ComponenteDto MapToDto(ComponenteEntity entity) => _mapper.ToDto(entity);
}
