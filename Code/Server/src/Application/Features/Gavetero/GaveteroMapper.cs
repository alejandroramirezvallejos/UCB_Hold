using Riok.Mapperly.Abstractions;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
namespace IMT_Reservas.Server.Application.Features.Gavetero;

[Mapper]
public partial class GaveteroMapper
{
    public partial GaveteroDto ToDto(GaveteroEntity entity);
    public partial GaveteroEntity ToEntity(GaveteroDto dto);
    public partial void Update(GaveteroDto source, GaveteroEntity destination);
}
