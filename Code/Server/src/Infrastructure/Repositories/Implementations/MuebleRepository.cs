using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Core.Common;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleEntity, MuebleDto>
{
    public MuebleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<MuebleDto>>> GetAll(QueryFilter? filter = null)
    {
        var entities = await DbContext.Muebles
            .AsNoTracking()
            .ToListAsync();
        return Result<List<MuebleDto>>.Success(entities.Select(MapToDto).ToList());
    }

    public override async Task<Result<MuebleDto>> Get(int id)
    {
        var entity = await DbContext.Muebles
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return entity == null
            ? Result<MuebleDto>.NotFound()
            : Result<MuebleDto>.Success(MapToDto(entity));
    }

    public async Task<MuebleEntity?> GetByNombre(string nombre)
        => await DbContext.Muebles.FirstOrDefaultAsync(m => m.Nombre == nombre && !m.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Muebles.AnyAsync(m => m.Id == id && !m.EstadoEliminado);

    protected override MuebleDto MapToDto(MuebleEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Ubicacion = entity.Ubicacion,
        NumeroGaveteros = entity.NumeroGaveteros,
        Tipo = entity.Tipo,
        Costo = entity.Costo,
        Longitud = entity.Longitud,
        Profundidad = entity.Profundidad,
        Altura = entity.Altura
    };
}


