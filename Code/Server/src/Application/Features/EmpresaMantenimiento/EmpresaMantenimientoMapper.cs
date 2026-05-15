using Riok.Mapperly.Abstractions;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

[Mapper]
public partial class EmpresaMantenimientoMapper
{
    [MapProperty(nameof(EmpresaMantenimientoEntity.Nombre), nameof(EmpresaMantenimientoDto.NombreEmpresa))]
    public partial EmpresaMantenimientoDto ToDto(EmpresaMantenimientoEntity entity);

    [MapProperty(nameof(EmpresaMantenimientoDto.NombreEmpresa), nameof(EmpresaMantenimientoEntity.Nombre))]
    public partial EmpresaMantenimientoEntity ToEntity(EmpresaMantenimientoDto dto);

    [MapProperty(nameof(EmpresaMantenimientoDto.NombreEmpresa), nameof(EmpresaMantenimientoEntity.Nombre))]
    public partial void Update(EmpresaMantenimientoDto source, EmpresaMantenimientoEntity destination);
}
