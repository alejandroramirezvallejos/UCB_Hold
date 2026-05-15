using Riok.Mapperly.Abstractions;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Application.Features.Mueble;

[Mapper]
public partial class MuebleMapper
{
    public partial MuebleDto ToDto(MuebleEntity entity);
    public partial MuebleEntity ToEntity(MuebleDto dto);
    public partial void Update(MuebleDto source, MuebleEntity destination);
}
