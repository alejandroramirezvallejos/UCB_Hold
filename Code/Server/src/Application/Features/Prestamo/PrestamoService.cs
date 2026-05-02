using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService
{
    private readonly PrestamoRepository _repository;
    private readonly ApplicationDbContext _dbContext;

    public PrestamoService(PrestamoRepository repository, ApplicationDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }

    public async Task<Result<PrestamoDetailDto>> Create(PrestamoEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<PrestamoDetailDto>.Error("Error al crear prestamo")
            : Result<PrestamoDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<PrestamoDetailDto>> Update(PrestamoEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<PrestamoDetailDto>.Error("Error al actualizar prestamo")
            : Result<PrestamoDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar prestamo");
    }

    public async Task<Result<PrestamoDetailDto>> Get(int id)
    {
        var prestamo = await _repository.Get(id);
        return !prestamo.IsSuccess
            ? Result<PrestamoDetailDto>.NotFound()
            : Result<PrestamoDetailDto>.Success(MapListDtoToDetailDto(prestamo.Value));
    }

    public async Task<Result<List<PrestamoListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<PrestamoListDto>>.Success(result.Value)
            : Result<List<PrestamoListDto>>.Error("Error al obtener prestamos");
    }

    public async Task<Result<Dictionary<int, int>>> GetAvailable(DateTime start, DateTime end, List<int> groupIds)
    {
        var result = new Dictionary<int, int>();
        foreach (var groupId in groupIds)
        {
            var available = await CountAvailableEquipo(groupId, start, end);
            result[groupId] = available;
        }
        return Result<Dictionary<int, int>>.Success(result);
    }

    public async Task<Result<List<DateTime>>> GetUnavailable(DateTime start, DateTime end, Dictionary<int, int> required)
    {
        var unavailable = new List<DateTime>();
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            foreach (var (groupId, qty) in required)
            {
                var available = await CountAvailableEquipo(groupId, date, date);
                if (available < qty)
                {
                    unavailable.Add(date);
                    break;
                }
            }
        }
        return Result<List<DateTime>>.Success(unavailable);
    }

    private static PrestamoDetailDto MapEntityToDetailDto(PrestamoEntity entity) => new()
    {
        Id = entity.Id,
        IdUsuario = 0,
        FechaSolicitud = entity.FechaSolicitud,
        FechaInicio = entity.FechaPrestamo ?? DateTime.Now,
        FechaFin = entity.FechaDevolucionEsperada,
        EstadoPrestamo = entity.EstadoPrestamo,
        Observaciones = entity.Observacion,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static PrestamoDetailDto MapListDtoToDetailDto(PrestamoListDto dto) => new()
    {
        Id = dto.Id,
        IdUsuario = 0,
        FechaSolicitud = dto.FechaSolicitud ?? DateTime.Now,
        FechaInicio = dto.FechaPrestamo ?? DateTime.Now,
        FechaFin = dto.FechaDevolucionEsperada ?? DateTime.Now,
        EstadoPrestamo = dto.EstadoPrestamo,
        Observaciones = dto.Observacion,
        EstadoEliminado = false
    };

    private async Task<int> CountAvailableEquipo(int groupId, DateTime start, DateTime end)
        => await _dbContext.Set<Core.Entities.Equipo>()
            .CountAsync(e => e.IdGrupoEquipo == groupId
                && e.EstadoEquipo == "operativo"
                && !e.EstadoEliminado);
}
