using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MongoDbContext _mongoDbContext;

    public PrestamoService(PrestamoRepository repository, ApplicationDbContext dbContext, MongoDbContext mongoDbContext)
        : base(repository) => (_dbContext, _mongoDbContext) = (dbContext, mongoDbContext);

    public override async Task<Result<PrestamoDto>> Create(PrestamoEntity entity)
    {
        var dateValidation = ValidateDates(entity.FechaPrestamo, entity.FechaDevolucionEsperada);

        if (!dateValidation.IsSuccess)
            return Result<PrestamoDto>.Error(dateValidation.Errors.FirstOrDefault() ?? "Error en fechas");

        var usuarioExists = await _dbContext.Usuarios.AnyAsync(u => u.Carnet == entity.Carnet && !u.EstadoEliminado);

        if (!usuarioExists)
            return Result<PrestamoDto>.Error("Usuario no existe o está inactivo");

        return await base.Create(entity);
    }

    public async Task<Result<bool>> ValidateDisponibilidad(int[] equipoIds, DateTime fechaInicio, DateTime fechaFin)
    {
        if (equipoIds == null || equipoIds.Length == 0)
            return Result<bool>.Error("Seleccione al menos un equipo");

        var invalidEquipos = await _dbContext.Equipos
            .Where(e => equipoIds.Contains(e.Id))
            .Where(e => e.EstadoEquipo != EstadoEquipo.Operativo || e.EstadoEliminado)
            .Select(e => e.Id)
            .ToListAsync();

        if (invalidEquipos.Any())
            return Result<bool>.Error("Equipos no disponibles");

        var prestamosEnRango = await _dbContext.Prestamos
            .Where(p => _dbContext.DetallesPrestamos
                .Where(d => equipoIds.Contains(d.IdEquipo))
                .Select(d => d.IdPrestamo)
                .Contains(p.Id))
            .Where(p => p.FechaPrestamo <= fechaFin && p.FechaDevolucion >= fechaInicio)
            .Where(p => p.EstadoPrestamo != EstadoPrestamo.Cancelado && p.EstadoPrestamo != EstadoPrestamo.Rechazado)
            .AnyAsync();

        if (prestamosEnRango)
            return Result<bool>.Error("Conflicto de fechas con otros préstamos");

        return Result<bool>.Success(true);
    }

    public Task<Result<object>> ValidateEstado(string estadoActual, string estadoNuevo)
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

        var equipos = await _dbContext.Equipos.Where(e => equipoIds.Contains(e.Id)).ToListAsync();
        var monto = equipos.Sum(e => e.CostoReferencia ?? 0);
        
        return Result<decimal>.Success((decimal)monto);
    }

    public async Task<Result<PrestamoDto>> UpdateEstado(int id, string nuevoEstado)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var estadoActual = prestamo.EstadoPrestamo.ToDbString();
        var validacion = await ValidateEstado(estadoActual, nuevoEstado);

        if (!validacion.IsSuccess)
            return Result<PrestamoDto>.Error(validacion.Errors.FirstOrDefault() ?? "Transición no permitida");

        var estadoParsed = nuevoEstado?.ToLowerInvariant() switch
        {
            "aprobado" => EstadoPrestamo.Aprobado,
            "activo" => EstadoPrestamo.Activo,
            "rechazado" => EstadoPrestamo.Rechazado,
            "finalizado" => EstadoPrestamo.Finalizado,
            "cancelado" => EstadoPrestamo.Cancelado,
            "pendiente" => EstadoPrestamo.Pendiente,
            _ => (EstadoPrestamo?)null
        };

        if (!estadoParsed.HasValue)
            return Result<PrestamoDto>.Error("Estado préstamo no válido");

        prestamo.EstadoPrestamo = estadoParsed.Value;
        _dbContext.Prestamos.Update(prestamo);
        await _dbContext.SaveChangesAsync();

        return await Get(id);
    }

    public bool NeedsContrato(decimal monto) => monto >= 1000;

    public async Task<Result<List<PrestamoDto>>> GetHistorial(string carnetUsuario, string estadoPrestamo)
    {
        if (string.IsNullOrEmpty(carnetUsuario))
            return Result<List<PrestamoDto>>.Error("Carnet requerido");

        var prestamos = await _dbContext.Prestamos.Where(p => p.Carnet == carnetUsuario).ToListAsync();

        if (!string.IsNullOrEmpty(estadoPrestamo) && estadoPrestamo != "todos")
        {
            var estado = estadoPrestamo.ToLowerInvariant() switch
            {
                "aprobado" => EstadoPrestamo.Aprobado,
                "activo" => EstadoPrestamo.Activo,
                "rechazado" => EstadoPrestamo.Rechazado,
                "finalizado" => EstadoPrestamo.Finalizado,
                "cancelado" => EstadoPrestamo.Cancelado,
                "pendiente" => EstadoPrestamo.Pendiente,
                _ => (EstadoPrestamo?)null
            };

            if (!estado.HasValue)
                return Result<List<PrestamoDto>>.Error("Estado préstamo no válido");

            prestamos = prestamos.Where(p => p.EstadoPrestamo == estado.Value).ToList();
        }

        var dtos = prestamos.Select(p => Repository.ConvertToDto(p)).ToList();

        return Result<List<PrestamoDto>>.Success(dtos);
    }

    public async Task<Result<PrestamoDto>> SaveContrato(int prestamoId, byte[] contratoBytes)
    {
        if (contratoBytes == null || contratoBytes.Length == 0)
            return Result<PrestamoDto>.Error("Contenido de contrato vacío");

        if (contratoBytes.Length > 5_000_000)
            return Result<PrestamoDto>.Error("Contrato excede tamaño máximo (5MB)");

        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == prestamoId);

        if (prestamo == null)
            return Result<PrestamoDto>.Error("Préstamo no encontrado");

        if (prestamo.IdContrato != null)
            return Result<PrestamoDto>.Error("Préstamo ya tiene contrato");

        var contratoBase64 = Convert.ToBase64String(contratoBytes);
        var contrato = new Core.Entities.Contrato
        {
            MongoId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            PrestamoId = prestamoId,
            ContenidoBase64 = contratoBase64,
            FechaCreacion = DateTime.UtcNow,
            EstadoEliminado = false
        };

        var coleccion = _mongoDbContext.GetContratos;
        await coleccion.InsertOneAsync(contrato);

        prestamo.IdContrato = contrato.MongoId;
        _dbContext.Prestamos.Update(prestamo);
        await _dbContext.SaveChangesAsync();

        return await Get(prestamoId);
    }

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
