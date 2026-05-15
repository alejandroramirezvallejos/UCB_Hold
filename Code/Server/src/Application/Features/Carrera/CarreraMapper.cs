using Riok.Mapperly.Abstractions;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Application.Features.Carrera;

[Mapper]
public partial class CarreraMapper
{
    public partial CarreraDto ToDto(CarreraEntity entity);
    public partial CarreraEntity ToEntity(CarreraDto dto);
    public partial void Update(CarreraDto source, CarreraEntity destination);
}
