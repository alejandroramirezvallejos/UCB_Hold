using Riok.Mapperly.Abstractions;
using IMT_Reservas.Server.Core.Entities;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Application.Features.Equipo;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
public partial class EquipoMapper
{
    public partial EquipoDto ToDto(EquipoEntity entity);
    public partial EquipoEntity ToEntity(EquipoDto dto);
    public partial void Update(EquipoDto source, EquipoEntity destination);

    private string EstadoEquipoToString(EstadoEquipo estado) => estado switch
    {
        EstadoEquipo.ParcialmenteOperativo => "parcialmente_operativo",
        EstadoEquipo.Inoperativo => "inoperativo",
        _ => "operativo"
    };

    private DateTime? DateOnlyToDateTime(DateOnly source) => source.ToDateTime(TimeOnly.MinValue);

    private DateOnly DateTimeToDateOnly(DateTime? source)
        => source.HasValue ? DateOnly.FromDateTime(source.Value) : DateOnly.MinValue;
}
