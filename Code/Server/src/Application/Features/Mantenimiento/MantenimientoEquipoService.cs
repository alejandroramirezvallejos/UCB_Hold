using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoEquipoService
{
    private readonly ApplicationDbContext _dbContext;

    public MantenimientoEquipoService(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<object>> ReconcileQty(int groupId)
    {
        var grupo = await _dbContext.Set<Core.Entities.GrupoEquipo>()
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.EstadoEliminado);

        if (grupo == null)
            return Result<object>.NotFound();

        var actualCount = await _dbContext.Set<Core.Entities.Equipo>()
            .CountAsync(e => e.IdGrupoEquipo == groupId && !e.EstadoEliminado);

        if (grupo.Cantidad != actualCount)
        {
            grupo.Cantidad = actualCount;
            await _dbContext.SaveChangesAsync();
        }

        return Result<object>.Success(null!);
    }

    public async Task<Result<object>> IncrementQty(int groupId)
    {
        var grupo = await _dbContext.Set<Core.Entities.GrupoEquipo>()
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.EstadoEliminado);

        if (grupo == null)
            return Result<object>.NotFound();

        grupo.Cantidad++;
        await _dbContext.SaveChangesAsync();
        return Result<object>.Success(null!);
    }

    public async Task<Result<object>> DecrementQty(int groupId)
    {
        var grupo = await _dbContext.Set<Core.Entities.GrupoEquipo>()
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.EstadoEliminado);

        if (grupo == null)
            return Result<object>.NotFound();

        if (grupo.Cantidad > 0)
            grupo.Cantidad--;

        await _dbContext.SaveChangesAsync();
        return Result<object>.Success(null!);
    }

    public async Task<Result<object>> RecalcCost()
    {
        var grupos = await _dbContext.Set<Core.Entities.GrupoEquipo>()
            .Where(g => !g.EstadoEliminado)
            .ToListAsync();

        foreach (var grupo in grupos)
        {
            var avg = await _dbContext.Set<Core.Entities.Equipo>()
                .Where(e => e.IdGrupoEquipo == grupo.Id && e.EstadoEquipo == "operativo" && !e.EstadoEliminado)
                .AverageAsync(e => (decimal?)e.CostoReferencia) ?? 0;

            grupo.CostoPromedio = avg;
        }

        await _dbContext.SaveChangesAsync();
        return Result<object>.Success(null!);
    }

    public async Task<Result<List<int>>> GetNeedsMaint()
    {
        var needsMaint = await _dbContext.Set<Core.Entities.Equipo>()
            .Where(e => !e.EstadoEliminado && (
                e.EstadoEquipo == "parcialmente_operativo" ||
                e.EstadoEquipo == "inoperativo"))
            .Select(e => e.Id)
            .ToListAsync();

        return Result<List<int>>.Success(needsMaint);
    }
}
