using Riok.Mapperly.Abstractions;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
namespace IMT_Reservas.Server.Application.Features.Usuario;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName, EnumMappingIgnoreCase = true)]
public partial class UsuarioMapper
{
    public partial UsuarioDto ToDto(UsuarioEntity entity);
    public partial UsuarioEntity ToEntity(UsuarioDto dto);
    public partial void Update(UsuarioDto source, UsuarioEntity destination);
}
