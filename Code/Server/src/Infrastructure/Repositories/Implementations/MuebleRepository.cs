using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Core.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleEntity, MuebleDto>
{
    public MuebleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<Result<List<MuebleDto>>> GetAll(QueryFilter? filter = null)
    {
        var dtos = await DbContext.Muebles
            .AsNoTracking()
            .Select(e => new MuebleDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Ubicacion = e.Ubicacion,
                NumeroGaveteros = e.NumeroGaveteros,
                Tipo = e.Tipo,
                Costo = e.Costo,
                Longitud = e.Longitud,
                Profundidad = e.Profundidad,
                Altura = e.Altura
            })
            .ToListAsync();
        
        return Result<List<MuebleDto>>.Success(dtos);
    }

    public override async Task<Result<MuebleDto>> Get(int id)
    {
        var dto = await DbContext.Muebles
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(e => new MuebleDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Ubicacion = e.Ubicacion,
                NumeroGaveteros = e.NumeroGaveteros,
                Tipo = e.Tipo,
                Costo = e.Costo,
                Longitud = e.Longitud,
                Profundidad = e.Profundidad,
                Altura = e.Altura
            })
            .FirstOrDefaultAsync();

        return dto == null
            ? Result<MuebleDto>.NotFound()
            : Result<MuebleDto>.Success(dto);
    }
    
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


