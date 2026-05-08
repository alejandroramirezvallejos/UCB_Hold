using AutoMapper;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;

namespace IMT_Reservas.Server.Application.Mapping;

public class GrupoEquipoProfile : Profile
{
    public GrupoEquipoProfile()
    {
        CreateMap<GrupoEquipoDto, GrupoEquipoEntity>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id ?? 0))
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Nombre ?? string.Empty))
            .ForMember(d => d.Modelo, o => o.MapFrom(s => s.Modelo ?? string.Empty))
            .ForMember(d => d.Marca, o => o.MapFrom(s => s.Marca ?? string.Empty))
            .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Descripcion ?? string.Empty))
            .ForMember(d => d.UrlImagen, o => o.MapFrom(s => s.UrlImagen ?? string.Empty))
            .ForMember(d => d.IdCategoria, o => o.MapFrom(s => s.IdCategoria ?? 0))
            .ForMember(d => d.Cantidad, o => o.MapFrom(s => s.Cantidad ?? 0))
            .ForMember(d => d.Categoria, o => o.Ignore())
            .ForMember(d => d.EstadoEliminado, o => o.Ignore());
    }
}
