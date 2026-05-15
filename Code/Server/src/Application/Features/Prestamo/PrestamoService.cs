using Ardalis.Result;
using FluentValidation;
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
    private readonly PrestamoMapper _mapper;
    private readonly IValidator<PrestamoDto> _validator;

    public PrestamoService(PrestamoRepository repository, ApplicationDbContext dbContext, PrestamoMapper mapper, IValidator<PrestamoDto> validator)
        : base(repository) => (_dbContext, _mapper, _validator) = (dbContext, mapper, validator);

    public async Task<Result<PrestamoDto>> Create(PrestamoDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<PrestamoDto>();

        if (!await _dbContext.Usuarios.AnyAsync(usuario => usuario.Carnet == dto.CarnetUsuario && !usuario.EstadoEliminado))
            return Result<PrestamoDto>.Error("Usuario no existe o está inactivo");

        var entity = _mapper.ToEntity(dto);
        entity.FechaSolicitud = dto.FechaSolicitud ?? DateTime.UtcNow;
        entity.FechaPrestamo = dto.FechaPrestamo ?? dto.FechaPrestamoEsperada;
        entity.FechaPrestamoEsperada = dto.FechaPrestamoEsperada ?? DateTime.UtcNow;
        entity.FechaDevolucionEsperada = dto.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7);
        entity.EstadoPrestamo = EstadoPrestamo.Pendiente;

        _dbContext.Prestamos.Add(entity);
        await _dbContext.SaveChangesAsync();

        await AssignEquipos(entity.Id, dto.GrupoEquipoId);
        await SaveContrato(entity, dto.Contrato);

        var all = await Repository.GetAll();
        var resultDto = all.Value.FirstOrDefault(prestamo => prestamo.Id == entity.Id);
        return Result<PrestamoDto>.Success(resultDto ?? Repository.ConvertToDto(entity));
    }

    public async Task<Result<PrestamoDto>> Update(int id, PrestamoDto dto)
    {
        var entity = await _dbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == id);
        if (entity == null) return Result<PrestamoDto>.NotFound();

        _mapper.Update(dto, entity);
        _dbContext.Prestamos.Update(entity);
        await _dbContext.SaveChangesAsync();

        return await Get(id);
    }

    public async Task<Result<PrestamoDto>> UpdateEstado(int id, string nuevoEstado)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == id);
        if (prestamo == null) return Result<PrestamoDto>.NotFound();

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
            if (!estado.HasValue) return Result<List<PrestamoDto>>.Error("Estado préstamo no válido");
        }

        return await Repository.GetHistorialWithDetalles(carnetUsuario, estado);
    }

    public override async Task<Result<object>> Delete(int id)
    {
        var detalles = await _dbContext.DetallesPrestamos
            .Where(detalle => detalle.IdPrestamo == id && !detalle.EstadoEliminado)
            .ToListAsync();

        foreach (var detalle in detalles) detalle.EstadoEliminado = true;
        if (detalles.Count > 0) await _dbContext.SaveChangesAsync();

        return await base.Delete(id);
    }

    private async Task AssignEquipos(int prestamoId, List<int>? grupoEquipoIds)
    {
        if (grupoEquipoIds == null || !grupoEquipoIds.Any()) return;

        var assignedEquipoIds = new List<int>();

        foreach (var groupId in grupoEquipoIds)
        {
            var loanedIds = await _dbContext.DetallesPrestamos
                .Join(_dbContext.Prestamos, detalle => detalle.IdPrestamo, prestamo => prestamo.Id, (detalle, prestamo) => new { detalle, prestamo })
                .Where(joinResult => joinResult.prestamo.EstadoPrestamo == EstadoPrestamo.Pendiente
                                  || joinResult.prestamo.EstadoPrestamo == EstadoPrestamo.Aprobado
                                  || joinResult.prestamo.EstadoPrestamo == EstadoPrestamo.Activo)
                .Select(joinResult => joinResult.detalle.IdEquipo)
                .ToListAsync();

            loanedIds.AddRange(assignedEquipoIds);

            var equipoDisponible = await _dbContext.Equipos
                .FirstOrDefaultAsync(equipo => equipo.IdGrupoEquipo == groupId
                    && !equipo.EstadoEliminado
                    && equipo.EstadoEquipo == EstadoEquipo.Operativo
                    && !loanedIds.Contains(equipo.Id));

            if (equipoDisponible == null) continue;

            _dbContext.DetallesPrestamos.Add(new DetallePrestamo
            {
                IdPrestamo = prestamoId,
                IdEquipo = equipoDisponible.Id,
                EstadoEliminado = false
            });
            assignedEquipoIds.Add(equipoDisponible.Id);
        }
        await _dbContext.SaveChangesAsync();
    }

    private async Task SaveContrato(PrestamoEntity entity, string? contratoHtml)
    {
        if (string.IsNullOrEmpty(contratoHtml)) return;

        var contrato = new Core.Entities.Contrato { ContratoHtml = contratoHtml };
        _dbContext.Contratos.Add(contrato);
        await _dbContext.SaveChangesAsync();

        entity.IdContrato = contrato.Id;
        _dbContext.Prestamos.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
