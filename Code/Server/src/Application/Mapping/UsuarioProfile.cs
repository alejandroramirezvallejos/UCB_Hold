using AutoMapper;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Core.Entities;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Application.Mapping;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<UsuarioDto, UsuarioEntity>()
            .ForMember(d => d.Carnet, o => o.MapFrom(s => s.Carnet ?? string.Empty))
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Nombre ?? string.Empty))
            .ForMember(d => d.ApellidoPaterno, o => o.MapFrom(s => s.ApellidoPaterno ?? string.Empty))
            .ForMember(d => d.ApellidoMaterno, o => o.MapFrom(s => s.ApellidoMaterno ?? string.Empty))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email ?? string.Empty))
            .ForMember(d => d.Contrasena, o => o.MapFrom(s => s.Contrasena ?? string.Empty))
            .ForMember(d => d.Telefono, o => o.MapFrom(s => s.Telefono ?? string.Empty))
            .ForMember(d => d.IdCarrera, o => o.MapFrom(s => s.IdCarrera ?? 0))
            .ForMember(d => d.Rol, o => o.MapFrom(s => ParseRol(s.Rol)))
            .ForMember(d => d.EstadoEliminado, o => o.Ignore());
    }

    private static TipoUsuario ParseRol(string? rol) =>
        rol?.ToLowerInvariant() switch
        {
            "docente" => TipoUsuario.Docente,
            "administrador" => TipoUsuario.Administrador,
            _ => TipoUsuario.Estudiante
        };
}
