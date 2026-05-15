using Riok.Mapperly.Abstractions;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Application.Features.Componente;

[Mapper]
public partial class ComponenteMapper
{
    public partial ComponenteEntity ToEntity(ComponenteDto dto);
}
