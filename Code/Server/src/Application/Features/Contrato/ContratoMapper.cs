using Riok.Mapperly.Abstractions;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Application.Features.Contrato;

[Mapper]
public partial class ContratoMapper
{
    public partial ContratoDto ToDto(ContratoEntity entity);
    public partial ContratoEntity ToEntity(ContratoDto dto);
    public partial void Update(ContratoDto source, ContratoEntity destination);
}
