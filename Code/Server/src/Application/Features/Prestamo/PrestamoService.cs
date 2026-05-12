using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoRepository, PrestamoDto>
{
    private readonly ApplicationDbContext _dbContext;
    public PrestamoService(PrestamoRepository repository, ApplicationDbContext dbContext)
        : base(repository) => _dbContext = dbContext;

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
    public async Task<Result<PrestamoDto>> CreateFromDto(PrestamoDto request)
    {
        var entity = new PrestamoEntity
        {
            Carnet = request.CarnetUsuario,
            FechaSolicitud = request.FechaSolicitud ?? DateTime.UtcNow,
            FechaPrestamo = request.FechaPrestamo ?? request.FechaPrestamoEsperada,
            FechaPrestamoEsperada = request.FechaPrestamoEsperada ?? DateTime.UtcNow,
            FechaDevolucion = request.FechaDevolucion,
            FechaDevolucionEsperada = request.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7),
            Observacion = request.Observacion,
            EstadoPrestamo = EstadoPrestamo.Pendiente,
            IdContrato = request.IdContrato
        };

        var dateValidation = ValidateDates(entity.FechaPrestamo, entity.FechaDevolucionEsperada);

        if (!dateValidation.IsSuccess)
            return Result<PrestamoDto>.Error(dateValidation.Errors.FirstOrDefault() ?? "Error en fechas");

        var usuarioExists = await _dbContext.Usuarios.AnyAsync(u => u.Carnet == entity.Carnet && !u.EstadoEliminado);

        if (!usuarioExists)
            return Result<PrestamoDto>.Error("Usuario no existe o estÃ¡ inactivo");

        _dbContext.Prestamos.Add(entity);
        await _dbContext.SaveChangesAsync();

        if (request.GrupoEquipoId != null && request.GrupoEquipoId.Any())
        {
            foreach (var groupId in request.GrupoEquipoId)
            {
                var prestadosIds = await _dbContext.DetallesPrestamos
                    .Join(_dbContext.Prestamos, dp => dp.IdPrestamo, p => p.Id, (dp, p) => new { dp, p })
                    .Where(x => x.p.EstadoPrestamo == EstadoPrestamo.Pendiente || x.p.EstadoPrestamo == EstadoPrestamo.Aprobado || x.p.EstadoPrestamo == EstadoPrestamo.Activo)
                    .Select(x => x.dp.IdEquipo)
                    .ToListAsync();

                var equipoDisponible = await _dbContext.Equipos
                    .FirstOrDefaultAsync(e => e.IdGrupoEquipo == groupId 
                        && !e.EstadoEliminado 
                        && e.EstadoEquipo == EstadoEquipo.Operativo
                        && !prestadosIds.Contains(e.Id));
                        
                if (equipoDisponible != null)
                {
                    _dbContext.DetallesPrestamos.Add(new Core.Entities.DetallePrestamo
                    {
                        IdPrestamo = entity.Id,
                        IdEquipo = equipoDisponible.Id,
                        EstadoEliminado = false
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        if (!string.IsNullOrEmpty(request.Contrato))
        {
            var contratoHtml = request.Contrato;
            var contrato = new Core.Entities.Contrato
            {
                ContratoHtml = contratoHtml
            };
            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();

            entity.IdContrato = contrato.Id;
            _dbContext.Prestamos.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        var all = await Repository.GetAll();
        var resultDto = all.Value.FirstOrDefault(p => p.Id == entity.Id);
        return Result<PrestamoDto>.Success(resultDto ?? Repository.ConvertToDto(entity));
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

    public async Task<Result<PrestamoDto>> UpdateEstado(int id, string nuevoEstado)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var estadoActual = prestamo.EstadoPrestamo switch
        {
            EstadoPrestamo.Aprobado   => "aprobado",
            EstadoPrestamo.Activo     => "activo",
            EstadoPrestamo.Rechazado  => "rechazado",
            EstadoPrestamo.Finalizado => "finalizado",
            EstadoPrestamo.Cancelado  => "cancelado",
            _                         => "pendiente"
        };
        var validacion = await ValidateEstado(estadoActual, nuevoEstado);

        if (!validacion.IsSuccess)
            return Result<PrestamoDto>.Error(validacion.Errors.FirstOrDefault() ?? "Transición no permitida");

        var estadoParsed = nuevoEstado.ToLowerInvariant() switch
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

    public async Task<Result<List<PrestamoDto>>> GetHistorial(string carnetUsuario, string estadoPrestamo)
    {
        if (string.IsNullOrEmpty(carnetUsuario))
            return Result<List<PrestamoDto>>.Error("Carnet requerido");

        EstadoPrestamo? estado = null;

        if (!string.IsNullOrEmpty(estadoPrestamo) && estadoPrestamo != "todos")
        {
            estado = estadoPrestamo.ToLowerInvariant() switch
            {
                "aprobado" => EstadoPrestamo.Aprobado,
                "activo" => EstadoPrestamo.Activo,
                "rechazado" => EstadoPrestamo.Rechazado,
                "finalizado" => EstadoPrestamo.Finalizado,
                "cancelado" => EstadoPrestamo.Cancelado,
                "pendiente" => EstadoPrestamo.Pendiente,
                _ => null
            };

            if (!estado.HasValue)
                return Result<List<PrestamoDto>>.Error("Estado préstamo no válido");
        }

        return await Repository.GetHistorialWithDetalles(carnetUsuario, estado);
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

        var contratoHtml = System.Text.Encoding.UTF8.GetString(contratoBytes);
        
        var contrato = new Core.Entities.Contrato
        {
            ContratoHtml = contratoHtml
        };

        _dbContext.Contratos.Add(contrato);
        await _dbContext.SaveChangesAsync();

        prestamo.IdContrato = contrato.Id;
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
