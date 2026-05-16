using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class EmpresaMantenimientoRepository : Repository<EmpresaMantenimientoEntity, EmpresaMantenimientoDto>
{
    public EmpresaMantenimientoRepository(ApplicationDbContext dbContext, EmpresaMantenimientoMapper mapper)
        : base(dbContext, mapper) { }

    public async Task<int?> FindIdByNombre(string nombre)
        => await DbContext.EmpresasMantenimiento
            .AsNoTracking()
            .Where(e => e.Nombre == nombre && !e.EstadoEliminado)
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsByNit(string nit, int? excludeId = null)
        => await DbContext.EmpresasMantenimiento
            .AnyAsync(e => e.Nit == nit && !e.EstadoEliminado && e.Id != excludeId);

    public override async Task<Result<object>> Delete(int id)
    {
        var entity = await DbContext.EmpresasMantenimiento
            .FirstOrDefaultAsync(e => e.Id == id && !e.EstadoEliminado);

        if (entity == null)
            return Result<object>.NotFound();

        entity.EstadoEliminado = true;
        DbContext.Update(entity);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(null!);
    }
}
