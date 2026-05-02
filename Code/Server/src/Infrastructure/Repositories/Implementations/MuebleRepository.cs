using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class MuebleRepository : Repository<MuebleEntity, MuebleListDto>
{
    public MuebleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<MuebleEntity?> GetByNombre(string nombre)
        => await DbContext.Muebles.FirstOrDefaultAsync(m => m.Nombre == nombre && !m.EstadoEliminado);

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Muebles.AnyAsync(m => m.Id == id && !m.EstadoEliminado);

    public async Task<Result<object>> UpdateGavCount(int idMueble, int increment)
    {
        var mueble = await DbContext.Muebles.FirstOrDefaultAsync(m => m.Id == idMueble && !m.EstadoEliminado);
        
        if (mueble == null)
            return Result<object>.NotFound();

        mueble.NumeroGaveteros = Math.Max(0, mueble.NumeroGaveteros + increment);
        await DbContext.SaveChangesAsync();
        
        return Result<object>.Success(null!);
    }

    protected override MuebleListDto MapToDto(MuebleEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Ubicacion = entity.Ubicacion,
        NumeroGaveteros = entity.NumeroGaveteros,
        Tipo = entity.Tipo,
        Costo = (decimal?)entity.Costo,
        Longitud = (decimal?)entity.Longitud,
        Profundidad = (decimal?)entity.Profundidad,
        Altura = (decimal?)entity.Altura
    };
}

