using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService : Service<MantenimientoEntity, MantenimientoRepository, MantenimientoDto>
{
    public MantenimientoService(MantenimientoRepository repository) : base(repository) { }
}
