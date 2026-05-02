using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;

namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService
{
    private readonly EquipoRepository _repository;

    public EquipoService(EquipoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EquipoDetailDto>> Create(EquipoEntity entity)
    {
        var result = await _repository.Create(entity);
        return !result.IsSuccess
            ? Result<EquipoDetailDto>.Error("Error al crear equipo")
            : Result<EquipoDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<EquipoDetailDto>> Update(EquipoEntity entity)
    {
        var result = await _repository.Update(entity);
        return !result.IsSuccess
            ? Result<EquipoDetailDto>.Error("Error al actualizar equipo")
            : Result<EquipoDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar equipo");
    }

    public async Task<Result<EquipoDetailDto>> Get(int id)
    {
        var equipo = await _repository.Get(id);
        return !equipo.IsSuccess
            ? Result<EquipoDetailDto>.NotFound()
            : Result<EquipoDetailDto>.Success(MapListDtoToDetailDto(equipo.Value));
    }

    public async Task<Result<List<EquipoListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<EquipoListDto>>.Success(result.Value)
            : Result<List<EquipoListDto>>.Error("Error al obtener equipos");
    }

    private static EquipoDetailDto MapEntityToDetailDto(EquipoEntity entity) => new()
    {
        Id = entity.Id,
        IdGrupoEquipo = entity.IdGrupoEquipo,
        CodigoImt = entity.CodigoImt,
        IdGavetero = entity.IdGavetero,
        Modelo = entity.Modelo,
        Marca = entity.Marca,
        CodigoUcb = entity.CodigoUcb,
        NumeroSerial = entity.NumeroSerial,
        EstadoEquipo = entity.EstadoEquipo,
        Ubicacion = entity.Ubicacion,
        CostoReferencia = entity.CostoReferencia,
        Descripcion = entity.Descripcion,
        TiempoMaximoPrestamo = entity.TiempoMaximoPrestamo,
        Procedencia = entity.Procedencia,
        EstadoEliminado = entity.EstadoEliminado
    };

    private static EquipoDetailDto MapListDtoToDetailDto(EquipoListDto dto) => new()
    {
        Id = dto.Id,
        IdGrupoEquipo = 0,
        CodigoImt = dto.CodigoImt ?? 0,
        IdGavetero = null,
        Modelo = dto.Modelo,
        Marca = dto.Marca,
        CodigoUcb = dto.CodigoUcb,
        NumeroSerial = dto.NumeroSerial,
        EstadoEquipo = dto.EstadoEquipo,
        Ubicacion = dto.Ubicacion,
        CostoReferencia = dto.CostoReferencia.HasValue ? (double)dto.CostoReferencia.Value : null,
        Descripcion = dto.Descripcion,
        TiempoMaximoPrestamo = dto.TiempoMaximoPrestamo,
        Procedencia = dto.Procedencia,
        EstadoEliminado = false
    };
}
