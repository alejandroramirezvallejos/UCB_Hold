using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService : Service<EquipoEntity, EquipoRepository, EquipoDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly EquipoMapper _mapper;

    public EquipoService(EquipoRepository repository, ApplicationDbContext dbContext, EquipoMapper mapper, IValidator<EquipoDto> validator)
        : base(repository, validator)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected override EquipoEntity MapToEntity(EquipoDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<EquipoDto>> Create(EquipoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<EquipoDto>();

        var entity = _mapper.ToEntity(dto);
        var maxCodigo = await _dbContext.Equipos.MaxAsync(equipo => (int?)equipo.CodigoImt) ?? 0;
        entity.CodigoImt = maxCodigo + 1;
        entity.FechaIngresoEquipo = DateOnly.FromDateTime(DateTime.Now);

        var result = await base.Create(entity);
        
        if (result.IsSuccess) 
            await RecalcGrupoStats(entity.IdGrupoEquipo);
        
        return result;
    }

    public async Task<Result<EquipoDto>> Update(int id, EquipoDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<EquipoDto>();

        var existing = await _dbContext.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(equipo => equipo.Id == id && !equipo.EstadoEliminado);

        if (existing == null) 
            return Result<EquipoDto>.NotFound();

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        entity.CodigoImt = existing.CodigoImt;
        entity.FechaIngresoEquipo = existing.FechaIngresoEquipo;
        entity.EstadoEliminado = existing.EstadoEliminado;

        var result = await base.Update(entity);
        
        if (!result.IsSuccess) 
            return result;

        await RecalcGrupoStats(entity.IdGrupoEquipo);
        if (existing.IdGrupoEquipo != entity.IdGrupoEquipo)
            await RecalcGrupoStats(existing.IdGrupoEquipo);

        return result;
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var existing = await _dbContext.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(equipo => equipo.Id == id);

        if (existing == null) return Result<object>.NotFound();

        var result = await base.Delete(id);
        
        if (result.IsSuccess) 
            await RecalcGrupoStats(existing.IdGrupoEquipo);
        
        return result;
    }

    private async Task RecalcGrupoStats(int idGrupoEquipo)
    {
        var grupo = await _dbContext.GruposEquipos
            .FirstOrDefaultAsync(grupoEquipo => grupoEquipo.Id == idGrupoEquipo);

        if (grupo == null) 
            return;

        var stats = await _dbContext.Equipos
            .Where(equipo => equipo.IdGrupoEquipo == idGrupoEquipo && !equipo.EstadoEliminado)
            .Select(equipo => new { equipo.CostoReferencia })
            .ToListAsync();

        grupo.Cantidad = stats.Count;
        grupo.CostoPromedio = stats.Count == 0
            ? 0
            : (decimal)(stats.Where(equipo => equipo.CostoReferencia.HasValue).Sum(equipo => equipo.CostoReferencia ?? 0) / Math.Max(1, stats.Count));

        await _dbContext.SaveChangesAsync();
    }
}
