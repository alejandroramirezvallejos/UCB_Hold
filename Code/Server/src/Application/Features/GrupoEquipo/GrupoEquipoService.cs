using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoRepository, GrupoEquipoDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GrupoEquipoMapper _mapper;

    public GrupoEquipoService(GrupoEquipoRepository repository, ApplicationDbContext dbContext, GrupoEquipoMapper mapper, IValidator<GrupoEquipoDto> validator)
        : base(repository, validator) { _dbContext = dbContext; _mapper = mapper; }

    protected override GrupoEquipoEntity MapToEntity(GrupoEquipoDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoDto dto)
    {
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<GrupoEquipoDto>();

        return await base.Create(MapToEntity(dto));
    }

    public async Task<Result<GrupoEquipoDto>> Update(int id, GrupoEquipoDto dto)
    {
        dto.Id = id;
        await ResolveCategoria(dto);
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<GrupoEquipoDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;
        
        return await base.Update(entity);
    }

    public async Task<Result<List<GrupoEquipoDto>>> Search(string? nombre = null, string? categoria = null)
        => Result<List<GrupoEquipoDto>>.Success(await Repository.Search(nombre, categoria));

    private async Task ResolveCategoria(GrupoEquipoDto dto)
    {
        if ((dto.IdCategoria ?? 0) > 0) 
            return;
        
        if (string.IsNullOrWhiteSpace(dto.NombreCategoria)) 
            return;

        var categoria = await _dbContext.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(categoria => categoria.Nombre == dto.NombreCategoria && !categoria.EstadoEliminado);

        dto.IdCategoria = categoria?.Id;
    }
}
