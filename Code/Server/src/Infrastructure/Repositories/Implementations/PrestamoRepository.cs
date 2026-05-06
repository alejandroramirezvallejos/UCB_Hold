using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoEntity, PrestamoDto>
{
    public PrestamoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExistsActive(int id)
        => await DbContext.Prestamos.AnyAsync(p => p.Id == id && !p.EstadoEliminado);

    public PrestamoDto ConvertToDto(PrestamoEntity entity) => MapToDto(entity);

    protected override PrestamoDto MapToDto(PrestamoEntity entity) => new()
    {
        Id = entity.Id,
        CarnetUsuario = entity.Carnet ?? "",
        EstadoPrestamo = entity.EstadoPrestamo.ToDbString(),
        FechaSolicitud = entity.FechaSolicitud,
        FechaDevolucionEsperada = entity.FechaDevolucionEsperada,
        NombreUsuario = null,
        ApellidoPaternoUsuario = null,
        TelefonoUsuario = null,
        FechaPrestamoEsperada = entity.FechaPrestamoEsperada,
        FechaPrestamo = entity.FechaPrestamo,
        FechaDevolucion = entity.FechaDevolucion,
        Observacion = entity.Observacion,
        IdContrato = entity.IdContrato,
        NombreGrupoEquipo = null,
        NombreGavetero = null
    };
}

