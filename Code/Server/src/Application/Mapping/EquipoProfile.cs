using AutoMapper;
using IMT_Reservas.Server.Application.Features.Equipo;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;

namespace IMT_Reservas.Server.Application.Mapping;

public class EquipoProfile : Profile
{
    public EquipoProfile()
    {
        CreateMap<EquipoDto, EquipoEntity>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id ?? 0))
            .ForMember(d => d.CodigoImt, o => o.MapFrom(s => s.CodigoImt ?? 0))
            .ForMember(d => d.IdGrupoEquipo, o => o.MapFrom(s => s.IdGrupoEquipo ?? 0))
            .ForMember(d => d.IdGavetero, o => o.MapFrom(s => s.IdGavetero))
            .ForMember(d => d.FechaIngresoEquipo, o => o.MapFrom(s =>
                s.FechaIngresoEquipo.HasValue
                    ? DateOnly.FromDateTime(s.FechaIngresoEquipo.Value)
                    : DateOnly.FromDateTime(DateTime.UtcNow)))
            .ForMember(d => d.EstadoEliminado, o => o.Ignore());
    }
}
