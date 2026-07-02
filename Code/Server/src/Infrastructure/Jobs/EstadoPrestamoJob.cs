using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;

namespace IMT_Reservas.Server.Infrastructure.Jobs;

public class EstadoPrestamoJob
{
    private readonly ApplicationDbContext _db;
    private readonly AuditLogService _audit;

    public EstadoPrestamoJob(ApplicationDbContext db, AuditLogService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task Execute()
    {
        var today = DateTime.UtcNow.Date;

        var atrasadosIds = await _db
            .Prestamos.Where(p =>
                p.EstadoPrestamo == EstadoPrestamo.Activo
                && p.FechaDevolucionEsperada.Date < today
                && !p.EstadoEliminado
            )
            .Select(p => p.Id)
            .ToListAsync();

        if (atrasadosIds.Count > 0)
        {
            await _db
                .Prestamos.Where(p => atrasadosIds.Contains(p.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.EstadoPrestamo, EstadoPrestamo.Atrasado)
                );

            foreach (var id in atrasadosIds)
                await _audit.Log(
                    AuditAccion.AtrasadoAutomatico,
                    nameof(PrestamoEntity),
                    id.ToString()
                );
        }

        var rechazadosIds = await _db
            .Prestamos.Where(p =>
                (
                    p.EstadoPrestamo == EstadoPrestamo.Pendiente
                    || p.EstadoPrestamo == EstadoPrestamo.Aprobado
                )
                && p.FechaPrestamoEsperada.Date < today
                && !p.EstadoEliminado
            )
            .Select(p => p.Id)
            .ToListAsync();

        if (rechazadosIds.Count > 0)
        {
            await _db
                .Prestamos.Where(p => rechazadosIds.Contains(p.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(p => p.EstadoPrestamo, EstadoPrestamo.Rechazado)
                );

            foreach (var id in rechazadosIds)
                await _audit.Log(
                    AuditAccion.Rechazar,
                    nameof(PrestamoEntity),
                    id.ToString(),
                    "Auto-rechazado por exceder fecha de inicio"
                );
        }
    }
}
