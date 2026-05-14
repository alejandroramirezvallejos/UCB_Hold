using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
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

        var usuarioExists = await _dbContext.Usuarios.AnyAsync(usuario => usuario.Carnet == entity.Carnet && !usuario.EstadoEliminado);

        if (!usuarioExists)
            return Result<PrestamoDto>.Error("Usuario no existe o está inactivo");

        return await base.Create(entity);
    }

    public async Task<Result<PrestamoDto>> CreateFromDto(PrestamoDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CarnetUsuario))
            return Result<PrestamoDto>.Error("Carnet de usuario requerido");

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

        var usuarioExists = await _dbContext.Usuarios.AnyAsync(usuario => usuario.Carnet == entity.Carnet && !usuario.EstadoEliminado);

        if (!usuarioExists)
            return Result<PrestamoDto>.Error("Usuario no existe o está inactivo");

        _dbContext.Prestamos.Add(entity);
        await _dbContext.SaveChangesAsync();

        if (request.GrupoEquipoId != null && request.GrupoEquipoId.Any())
        {
            var assignedEquipoIds = new List<int>();

            foreach (var groupId in request.GrupoEquipoId)
            {
                var loanedIds = await _dbContext.DetallesPrestamos
                    .Join(_dbContext.Prestamos, detalle => detalle.IdPrestamo, prestamo => prestamo.Id, (detalle, prestamo) => new { detalle, prestamo })
                    .Where(x => x.prestamo.EstadoPrestamo == EstadoPrestamo.Pendiente || x.prestamo.EstadoPrestamo == EstadoPrestamo.Aprobado || x.prestamo.EstadoPrestamo == EstadoPrestamo.Activo)
                    .Select(x => x.detalle.IdEquipo)
                    .ToListAsync();

                loanedIds.AddRange(assignedEquipoIds);

                var equipoDisponible = await _dbContext.Equipos
                    .FirstOrDefaultAsync(equipo => equipo.IdGrupoEquipo == groupId
                        && !equipo.EstadoEliminado
                        && equipo.EstadoEquipo == EstadoEquipo.Operativo
                        && !loanedIds.Contains(equipo.Id));

                if (equipoDisponible != null)
                {
                    _dbContext.DetallesPrestamos.Add(new DetallePrestamo
                    {
                        IdPrestamo = entity.Id,
                        IdEquipo = equipoDisponible.Id,
                        EstadoEliminado = false
                    });
                    assignedEquipoIds.Add(equipoDisponible.Id);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        if (!string.IsNullOrEmpty(request.Contrato))
        {
            var contrato = new Core.Entities.Contrato
            {
                ContratoHtml = request.Contrato
            };
            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();

            entity.IdContrato = contrato.Id;
            _dbContext.Prestamos.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        var all = await Repository.GetAll();
        var resultDto = all.Value.FirstOrDefault(prestamo => prestamo.Id == entity.Id);
        return Result<PrestamoDto>.Success(resultDto ?? Repository.ConvertToDto(entity));
    }

    public async Task<Result<PrestamoDto>> UpdateEstado(int id, string nuevoEstado)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == id);

        if (prestamo == null)
            return Result<PrestamoDto>.NotFound();

        var parsedState = EstadoPrestamoState.Parse(nuevoEstado);

        if (!parsedState.HasValue)
            return Result<PrestamoDto>.Error($"Estado '{nuevoEstado}' no reconocido");

        if (!EstadoPrestamoState.CanTransition(prestamo.EstadoPrestamo, parsedState.Value))
            return Result<PrestamoDto>.Error($"Transición '{EstadoPrestamoState.ToText(prestamo.EstadoPrestamo)}' → '{nuevoEstado}' no permitida");

        prestamo.EstadoPrestamo = parsedState.Value;
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
            estado = EstadoPrestamoState.Parse(estadoPrestamo);

            if (!estado.HasValue)
                return Result<List<PrestamoDto>>.Error("Estado préstamo no válido");
        }

        return await Repository.GetHistorialWithDetalles(carnetUsuario, estado);
    }

    private Result<object> ValidateDates(DateTime? fechaPrestamo, DateTime? fechaDevolucion)
    {
        if (!fechaPrestamo.HasValue || !fechaDevolucion.HasValue)
            return Result<object>.Error("Fechas requeridas");

        if (fechaPrestamo.Value > fechaDevolucion.Value)
            return Result<object>.Error("Fecha préstamo no puede ser posterior a devolución");

        if (fechaPrestamo.Value < DateTime.Now.AddDays(-1))
            return Result<object>.Error("Fecha no puede ser retroactiva");

        return Result<object>.Success(null!);
    }
}
