using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService : Service<EquipoEntity, EquipoRepository, EquipoDto>
{
    private readonly ApplicationDbContext _dbContext;

    public EquipoService(EquipoRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _dbContext = dbContext;
    }

    public override async Task<Result<EquipoDto>> Create(EquipoEntity entity)
    {
        var grupoExists = await _dbContext.GruposEquipos
            .AnyAsync(g => g.Id == entity.IdGrupoEquipo && !g.EstadoEliminado);
        
        if (!grupoExists)
            return Result<EquipoDto>.Error("Grupo equipo no existe");

        if (entity.IdGavetero.HasValue)
        {
            var gaveteroExists = await _dbContext.Gaveteros
                .AnyAsync(g => g.Id == entity.IdGavetero && !g.EstadoEliminado);

            if (!gaveteroExists)
                return Result<EquipoDto>.Error("Gavetero no existe");
        }

        // Auto-generate unique CodigoImt
        var maxCodigo = await _dbContext.Equipos.MaxAsync(e => (int?)e.CodigoImt) ?? 0;
        entity.CodigoImt = maxCodigo + 1;

        return await base.Create(entity);
    }

    public override async Task<Result<EquipoDto>> Update(EquipoEntity entity)
    {
        var existing = await _dbContext.Equipos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entity.Id && !e.EstadoEliminado);

        if (existing == null)
            return Result<EquipoDto>.NotFound();

        // Preserve fields not editable by user
        entity.CodigoImt = existing.CodigoImt;
        entity.FechaIngresoEquipo = existing.FechaIngresoEquipo;
        entity.EstadoEliminado = existing.EstadoEliminado;

        return await base.Update(entity);
    }
}
