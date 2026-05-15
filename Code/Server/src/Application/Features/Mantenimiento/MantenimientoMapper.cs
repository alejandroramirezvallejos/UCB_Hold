using Riok.Mapperly.Abstractions;
using IMT_Reservas.Server.Application.Abstraction;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

[Mapper]
public partial class MantenimientoMapper : IMapper<MantenimientoEntity, MantenimientoDto>
{
    [MapperIgnoreTarget(nameof(MantenimientoDto.NombreEmpresaMantenimiento))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.NombreGrupoEquipo))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.CodigoImtEquipo))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.TipoMantenimiento))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.DescripcionEquipo))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.CodigoIMT))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.TiposMantenimiento))]
    [MapperIgnoreTarget(nameof(MantenimientoDto.DescripcionesEquipo))]
    public partial MantenimientoDto ToDto(MantenimientoEntity entity);

    [MapperIgnoreSource(nameof(MantenimientoDto.NombreEmpresaMantenimiento))]
    [MapperIgnoreSource(nameof(MantenimientoDto.NombreGrupoEquipo))]
    [MapperIgnoreSource(nameof(MantenimientoDto.CodigoImtEquipo))]
    [MapperIgnoreSource(nameof(MantenimientoDto.TipoMantenimiento))]
    [MapperIgnoreSource(nameof(MantenimientoDto.DescripcionEquipo))]
    [MapperIgnoreSource(nameof(MantenimientoDto.CodigoIMT))]
    [MapperIgnoreSource(nameof(MantenimientoDto.TiposMantenimiento))]
    [MapperIgnoreSource(nameof(MantenimientoDto.DescripcionesEquipo))]
    public partial MantenimientoEntity ToEntity(MantenimientoDto dto);

    public partial IQueryable<MantenimientoDto> ProjectTo(IQueryable<MantenimientoEntity> source);
}
