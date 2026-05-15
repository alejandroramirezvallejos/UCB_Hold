using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    public GrupoEquipoService(GrupoEquipoRepository repository, GrupoEquipoMapper mapper, IValidator<GrupoEquipoDto> validator)
        : base(repository, validator, mapper) { }

    public override async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<GrupoEquipoDto>();

        return await CreateEntity(MapToEntity(dto));
    }

    public override async Task<Result<GrupoEquipoDto>> Update(int id, GrupoEquipoDto dto)
    {
        dto.Id = id;
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<GrupoEquipoDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;

        return await UpdateEntity(entity);
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
        => Result<List<GrupoEquipoDto>>.Success(await Repository.Search(nombre, categoria));

    private async Task ResolveCategoria(GrupoEquipoDto dto)
    {
        if ((dto.IdCategoria ?? 0) > 0)
            return;

        if (string.IsNullOrWhiteSpace(dto.NombreCategoria))
            return;

        dto.IdCategoria = await Repository.FindCategoriaIdByNombre(dto.NombreCategoria);
    }
}
