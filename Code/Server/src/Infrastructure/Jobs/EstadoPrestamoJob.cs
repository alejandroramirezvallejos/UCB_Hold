using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Jobs;

public class EstadoPrestamoJob
{
    private readonly ApplicationDbContext _db;

    public EstadoPrestamoJob(ApplicationDbContext db) => _db = db;

    public async Task Execute()
    {
        var today = DateTime.UtcNow.Date;

        await _db.Prestamos
            .Where(p => p.EstadoPrestamo == EstadoPrestamo.Activo
                     && p.FechaDevolucionEsperada.Date < today
                     && !p.EstadoEliminado)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.EstadoPrestamo, EstadoPrestamo.Atrasado));

        await _db.Prestamos
            .Where(p => (p.EstadoPrestamo == EstadoPrestamo.Pendiente
                      || p.EstadoPrestamo == EstadoPrestamo.Aprobado)
                     && p.FechaPrestamoEsperada.Date < today
                     && !p.EstadoEliminado)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.EstadoPrestamo, EstadoPrestamo.Rechazado));
    }
}
