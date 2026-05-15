using Riok.Mapperly.Abstractions;
using IMT_Reservas.Server.Application.Abstraction;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Application.Features.Componente;

[Mapper]
public partial class ComponenteMapper : IMapper<ComponenteEntity, ComponenteDto>
{
    [MapperIgnoreTarget(nameof(ComponenteDto.NombreEquipo))]
    [MapperIgnoreTarget(nameof(ComponenteDto.CodigoImtEquipo))]
    public partial ComponenteDto ToDto(ComponenteEntity entity);

    [MapperIgnoreSource(nameof(ComponenteDto.NombreEquipo))]
    [MapperIgnoreSource(nameof(ComponenteDto.CodigoImtEquipo))]
    public partial ComponenteEntity ToEntity(ComponenteDto dto);

    public partial IQueryable<ComponenteDto> ProjectTo(IQueryable<ComponenteEntity> source);
}
