using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using Riok.Mapperly.Abstractions;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;

namespace IMT_Reservas.Server.Application.Features.Usuario;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
public partial class UsuarioMapper : IMapper<UsuarioEntity, UsuarioDto>
{
    [MapperIgnoreTarget(nameof(UsuarioDto.Contrasena))]
    [MapperIgnoreTarget(nameof(UsuarioDto.CarreraNombre))]
    public partial UsuarioDto ToDto(UsuarioEntity entity);

    public partial UsuarioEntity ToEntity(UsuarioDto dto);

    public partial void Update(UsuarioDto source, UsuarioEntity destination);

    public partial IQueryable<UsuarioDto> ProjectTo(IQueryable<UsuarioEntity> source);

    private static string TipoUsuarioToString(TipoUsuario rol) => rol.ToString().ToLowerInvariant();
}
