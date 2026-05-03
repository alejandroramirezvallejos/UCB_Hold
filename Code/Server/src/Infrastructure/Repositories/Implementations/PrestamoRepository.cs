using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class PrestamoRepository : Repository<PrestamoEntity, PrestamoDto>
{
    public PrestamoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<bool> ExisteActivoPorId(int id)
        => await DbContext.Prestamos.AnyAsync(p => p.Id == id && !p.EstadoEliminado);

    public async Task<IEnumerable<PrestamoEntity>> GetByCarnet(string carnet)
        => await DbContext.Prestamos
            .Where(p => p.Carnet == carnet && !p.EstadoEliminado)
            .ToListAsync();

    public async Task<IEnumerable<PrestamoEntity>> GetByState(string estadoPrestamo)
        => await DbContext.Prestamos
            .Where(p => p.EstadoPrestamo == estadoPrestamo && !p.EstadoEliminado)
            .ToListAsync();

    protected override PrestamoDto MapToDto(PrestamoEntity entity) => new()
    {
        Id = entity.Id,
        CarnetUsuario = entity.Carnet ?? "",
        EstadoPrestamo = entity.EstadoPrestamo,
        FechaSolicitud = entity.FechaSolicitud,
        FechaDevolucionEsperada = entity.FechaDevolucionEsperada,
        NombreUsuario = null,
        ApellidoPaternoUsuario = null,
        TelefonoUsuario = null,
        CodigoImt = null,
        FechaPrestamoEsperada = entity.FechaPrestamoEsperada,
        FechaPrestamo = entity.FechaPrestamo,
        FechaDevolucion = entity.FechaDevolucion,
        Observacion = entity.Observacion,
        Ubicacion_Equipo = null,
        Nombre_Mueble = null,
        Ubicacion_Mueble = null,
        IdContrato = int.TryParse(entity.IdContrato, out var id) ? id : null,
        NombreGrupoEquipo = null,
        Nombre_Gavetero = null
    };
}

