using IMT_Reservas.Server.Application.Abstraction;
using Riok.Mapperly.Abstractions;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;

namespace IMT_Reservas.Server.Application.Features.Mueble;

[Mapper]
public partial class MuebleMapper : IMapper<MuebleEntity, MuebleDto>
{
    public partial MuebleDto ToDto(MuebleEntity entity);

    public partial MuebleEntity ToEntity(MuebleDto dto);

    public partial IQueryable<MuebleDto> ProjectTo(IQueryable<MuebleEntity> source);
}
