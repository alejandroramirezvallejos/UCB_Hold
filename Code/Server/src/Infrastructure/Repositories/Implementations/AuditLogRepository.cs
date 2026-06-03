using IMT_Reservas.Server.Application.Features.AuditLog;
using IMT_Reservas.Server.Infrastructure.Config;
using AuditLogEntity = IMT_Reservas.Server.Core.Entities.AuditLog;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class AuditLogRepository
{
    private readonly ApplicationDbContext _db;

    public AuditLogRepository(ApplicationDbContext db) => _db = db;

    public async Task WriteLog(AuditLogEntity entry)
    {
        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AuditLogDto>> GetFiltered(
        string? entidad,
        string? carnet,
        DateTime? desde,
        DateTime? hasta)
    {
        var query = _db.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(entidad)) 
            query = query.Where(a => a.Entidad == entidad);
        if (!string.IsNullOrWhiteSpace(carnet))  
            query = query.Where(a => a.AdminCarnet == carnet);
        if (desde.HasValue) 
            query = query.Where(a => a.Timestamp >= desde.Value.ToUniversalTime());
        if (hasta.HasValue) 
            query = query.Where(a => a.Timestamp <= hasta.Value.ToUniversalTime());

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Take(300)
            .Select(a => new AuditLogDto
            {
                Id          = a.Id,
                AdminCarnet = a.AdminCarnet,
                AdminNombre = a.AdminNombre,
                Accion      = a.Accion,
                Entidad     = a.Entidad,
                EntidadId   = a.EntidadId,
                Detalle     = a.Detalle,
                Timestamp   = a.Timestamp
            })
            .ToListAsync();
    }
}
