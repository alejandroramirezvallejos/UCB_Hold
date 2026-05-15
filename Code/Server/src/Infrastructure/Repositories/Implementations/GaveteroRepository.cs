using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class GaveteroRepository : Repository<GaveteroEntity, GaveteroDto>
{
    private readonly GaveteroMapper _mapper;

    public GaveteroRepository(ApplicationDbContext dbContext, GaveteroMapper mapper)
        : base(dbContext)
    {
        _mapper = mapper;
    }

    protected override GaveteroDto MapToDto(GaveteroEntity entity) => _mapper.ToDto(entity);

    public override async Task<Result<List<GaveteroDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Gaveteros
            .AsNoTracking()
            .Select(e => new GaveteroDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Tipo = e.Tipo,
                NombreMueble = null,
                Longitud = e.Longitud,
                Profundidad = e.Profundidad,
                Altura = e.Altura
            })
            .ToListAsync();
        
        return Result<List<GaveteroDto>>.Success(dtos);
    }

    public override async Task<Result<GaveteroDto>> Get(int id)
    {
        var entity = await DbContext.Gaveteros
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id && !g.EstadoEliminado);

        return entity == null
            ? Result<GaveteroDto>.NotFound()
            : Result<GaveteroDto>.Success(MapToDto(entity));
    }
    
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

}

