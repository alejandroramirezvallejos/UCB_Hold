using Riok.Mapperly.Abstractions;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

[Mapper]
public partial class MantenimientoMapper
{
    public partial MantenimientoDto ToDto(MantenimientoEntity entity);
    public partial MantenimientoEntity ToEntity(MantenimientoDto dto);
    public partial void Update(MantenimientoDto source, MantenimientoEntity destination);
}
