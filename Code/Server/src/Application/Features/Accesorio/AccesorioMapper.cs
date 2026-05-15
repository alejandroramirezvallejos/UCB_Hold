using Riok.Mapperly.Abstractions;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

[Mapper]
public partial class AccesorioMapper
{
    public partial AccesorioDto ToDto(AccesorioEntity entity);
    public partial AccesorioEntity ToEntity(AccesorioDto dto);
    public partial void Update(AccesorioDto source, AccesorioEntity destination);
}
