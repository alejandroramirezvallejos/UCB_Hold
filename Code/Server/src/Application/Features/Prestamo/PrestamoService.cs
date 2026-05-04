using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly PrestamoRepository _repository;
    private readonly ApplicationDbContext _dbContext;

    public PrestamoService(PrestamoRepository repository, ApplicationDbContext dbContext)
        : base(repository)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public override async Task<Result<PrestamoDto>> Create(PrestamoEntity entity)
    {
        var dateValidation = ValidateDates(entity.FechaPrestamo, entity.FechaDevolucionEsperada);
        if (!dateValidation.IsSuccess)
            return Result<PrestamoDto>.Error(dateValidation.Errors.FirstOrDefault() ?? "Error en fechas");

        var usuarioExists = await _dbContext.Usuarios
            .AnyAsync(u => u.Carnet == entity.Carnet && !u.EstadoEliminado);
        
        if (!usuarioExists)
            return Result<PrestamoDto>.Error("Usuario no existe o está inactivo");

        var activePrestamoExists = await _dbContext.Prestamos
            .AnyAsync(p => p.Carnet == entity.Carnet
                        && (p.EstadoPrestamo == "activo" || p.EstadoPrestamo == "pendiente")
                        && !p.EstadoEliminado);
       
        if (activePrestamoExists)
            return Result<PrestamoDto>.Error("Usuario tiene préstamo activo o pendiente");

        return await base.Create(entity);
    }

    public async Task<Result<bool>> ValidateAvailability(int[] equipoIds, DateTime fechaInicio, DateTime fechaFin)
    {
        if (equipoIds == null || equipoIds.Length == 0)
            return Result<bool>.Error("Seleccione al menos un equipo");

        var invalidEquipos = await _dbContext.Equipos
            .Where(e => equipoIds.Contains(e.Id))
            .Where(e => e.EstadoEquipo != "operativo" || e.EstadoEliminado)
            .Select(e => e.Id)
            .ToListAsync();

        if (invalidEquipos.Any())
            return Result<bool>.Error($"Equipos no disponibles");

        var prestamosEnRango = await _dbContext.Prestamos
            .Where(p => _dbContext.DetallesPrestamos
                .Where(d => equipoIds.Contains(d.IdEquipo))
                .Select(d => d.IdPrestamo)
                .Contains(p.Id))
            .Where(p => p.FechaPrestamo <= fechaFin && p.FechaDevolucion >= fechaInicio)
            .Where(p => p.EstadoPrestamo != "cancelado" && p.EstadoPrestamo != "rechazado")
            .AnyAsync();

        if (prestamosEnRango)
            return Result<bool>.Error("Conflicto de fechas con otros préstamos");

        return Result<bool>.Success(true);
    }

    public Task<Result<object>> ValidateStateTransition(string estadoActual, string estadoNuevo)
    {
        var estadosValidos = new[] { "pendiente", "rechazado", "aprobado", "activo", "finalizado", "cancelado" };

        if (!estadosValidos.Contains(estadoNuevo))
            return Task.FromResult(Result<object>.Error($"Estado '{estadoNuevo}' no válido"));

        var transiciones = new Dictionary<string, string[]>
        {
            { "pendiente", new[] { "aprobado", "rechazado", "cancelado" } },
            { "aprobado", new[] { "activo", "cancelado" } },
            { "activo", new[] { "finalizado", "cancelado" } },
            { "rechazado", Array.Empty<string>() },
            { "finalizado", Array.Empty<string>() },
            { "cancelado", Array.Empty<string>() }
        };

        if (transiciones.TryGetValue(estadoActual, out var validos) && !validos.Contains(estadoNuevo))
            return Task.FromResult(Result<object>.Error("Transición no permitida"));

        return Task.FromResult(Result<object>.Success(null!));
    }

    public async Task<Result<decimal>> CalculateMonto(int[] equipoIds)
    {
        if (equipoIds == null || equipoIds.Length == 0)
            return Result<decimal>.Success(0);

        var equipos = await _dbContext.Equipos
            .Where(e => equipoIds.Contains(e.Id))
            .ToListAsync();

        var monto = equipos.Sum(e => e.CostoReferencia ?? 0);
        return Result<decimal>.Success((decimal)monto);
    }

    public bool RequiereContrato(decimal monto) => monto > 1000;

    private Result<object> ValidateDates(DateTime? fechaPrestamo, DateTime? fechaDevolucion)
    {
        if (!fechaPrestamo.HasValue || !fechaDevolucion.HasValue)
            return Result<object>.Error("Fechas requeridas");

        if (fechaPrestamo.Value >= fechaDevolucion.Value)
            return Result<object>.Error("Fecha préstamo menor a devolución");

        if (fechaPrestamo.Value < DateTime.Now.AddDays(-1))
            return Result<object>.Error("Fecha no puede ser retroactiva");

        return Result<object>.Success(null!);
    }
}
