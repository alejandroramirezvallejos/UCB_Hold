using Riok.Mapperly.Abstractions;
using IMT_Reservas.Server.Core.Entities;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
public partial class PrestamoMapper
{
    [MapProperty(nameof(PrestamoEntity.Carnet), nameof(PrestamoDto.CarnetUsuario))]
    public partial PrestamoDto ToDto(PrestamoEntity entity);

    [MapProperty(nameof(PrestamoDto.CarnetUsuario), nameof(PrestamoEntity.Carnet))]
    public partial PrestamoEntity ToEntity(PrestamoDto dto);

    [MapProperty(nameof(PrestamoDto.CarnetUsuario), nameof(PrestamoEntity.Carnet))]
    public partial void Update(PrestamoDto source, PrestamoEntity destination);

    private string EstadoPrestamoToString(EstadoPrestamo estado) => EstadoPrestamoState.ToText(estado);
}
