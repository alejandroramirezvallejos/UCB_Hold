using Riok.Mapperly.Abstractions;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

[Mapper]
public partial class GrupoEquipoMapper
{
    public partial GrupoEquipoDto ToDto(GrupoEquipoEntity entity);
    public partial GrupoEquipoEntity ToEntity(GrupoEquipoDto dto);
}
