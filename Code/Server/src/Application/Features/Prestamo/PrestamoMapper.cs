using Riok.Mapperly.Abstractions;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
public partial class PrestamoMapper : IMapper<PrestamoEntity, PrestamoDto>
{
    [MapProperty(nameof(PrestamoEntity.Carnet), nameof(PrestamoDto.CarnetUsuario))]
    [MapperIgnoreTarget(nameof(PrestamoDto.NombreUsuario))]
    [MapperIgnoreTarget(nameof(PrestamoDto.ApellidoPaternoUsuario))]
    [MapperIgnoreTarget(nameof(PrestamoDto.TelefonoUsuario))]
    [MapperIgnoreTarget(nameof(PrestamoDto.NombreGrupoEquipo))]
    [MapperIgnoreTarget(nameof(PrestamoDto.CodigoImt))]
    [MapperIgnoreTarget(nameof(PrestamoDto.UbicacionEquipo))]
    [MapperIgnoreTarget(nameof(PrestamoDto.NombreGavetero))]
    [MapperIgnoreTarget(nameof(PrestamoDto.NombreMueble))]
    [MapperIgnoreTarget(nameof(PrestamoDto.UbicacionMueble))]
    [MapperIgnoreTarget(nameof(PrestamoDto.GrupoEquipoId))]
    [MapperIgnoreTarget(nameof(PrestamoDto.Contrato))]
    public partial PrestamoDto ToDto(PrestamoEntity entity);

    [MapProperty(nameof(PrestamoDto.CarnetUsuario), nameof(PrestamoEntity.Carnet))]
    [MapperIgnoreSource(nameof(PrestamoDto.NombreUsuario))]
    [MapperIgnoreSource(nameof(PrestamoDto.ApellidoPaternoUsuario))]
    [MapperIgnoreSource(nameof(PrestamoDto.TelefonoUsuario))]
    [MapperIgnoreSource(nameof(PrestamoDto.NombreGrupoEquipo))]
    [MapperIgnoreSource(nameof(PrestamoDto.CodigoImt))]
    [MapperIgnoreSource(nameof(PrestamoDto.UbicacionEquipo))]
    [MapperIgnoreSource(nameof(PrestamoDto.NombreGavetero))]
    [MapperIgnoreSource(nameof(PrestamoDto.NombreMueble))]
    [MapperIgnoreSource(nameof(PrestamoDto.UbicacionMueble))]
    [MapperIgnoreSource(nameof(PrestamoDto.GrupoEquipoId))]
    [MapperIgnoreSource(nameof(PrestamoDto.Contrato))]
    public partial PrestamoEntity ToEntity(PrestamoDto dto);

    [MapProperty(nameof(PrestamoDto.CarnetUsuario), nameof(PrestamoEntity.Carnet))]
    public partial void Update(PrestamoDto source, PrestamoEntity destination);

    public partial IQueryable<PrestamoDto> ProjectTo(IQueryable<PrestamoEntity> source);

    private static string EstadoPrestamoToString(EstadoPrestamo estado) => EstadoPrestamoState.ToText(estado);
}
