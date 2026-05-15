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
}
