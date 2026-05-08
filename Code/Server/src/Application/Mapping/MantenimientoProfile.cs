using AutoMapper;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Application.Mapping;

public class MantenimientoProfile : Profile
{
    public MantenimientoProfile()
    {
        CreateMap<MantenimientoDto, MantenimientoEntity>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id ?? 0))
            .ForMember(d => d.IdEmpresa, o => o.MapFrom(s => s.IdEmpresa ?? 0))
            .ForMember(d => d.FechaMantenimiento, o => o.MapFrom(s => s.FechaMantenimiento ?? DateTime.UtcNow))
            .ForMember(d => d.FechaFinalMantenimiento, o => o.MapFrom(s => s.FechaFinalMantenimiento ?? DateTime.UtcNow))
            .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Descripcion))
            .ForMember(d => d.Costo, o => o.MapFrom(s => s.Costo))
            .ForMember(d => d.EstadoEliminado, o => o.Ignore());
    }
}
