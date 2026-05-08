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

        return await base.Create(entity);
    }

    public override async Task<Result<EquipoDto>> Update(EquipoEntity entity)
        => await base.Update(entity);
}
