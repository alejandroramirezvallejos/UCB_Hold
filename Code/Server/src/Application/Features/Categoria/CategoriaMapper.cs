using Riok.Mapperly.Abstractions;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
namespace IMT_Reservas.Server.Application.Features.Categoria;

[Mapper]
public partial class CategoriaMapper
{
    public partial CategoriaDto ToDto(CategoriaEntity entity);
    public partial CategoriaEntity ToEntity(CategoriaDto dto);
}
