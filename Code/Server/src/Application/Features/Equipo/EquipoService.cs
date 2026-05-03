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

    public async Task<Result<EquipoDetail>> Create(EquipoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<EquipoDetail>.Error("Error al crear equipo")
            : Result<EquipoDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<EquipoDetail>> Update(EquipoEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<EquipoDetail>.Error("Error al actualizar equipo")
            : Result<EquipoDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar equipo");
    }

    public async Task<Result<EquipoDetail>> Get(int id)
    {
        var equipo = await _repository.Get(id);
        
        return !equipo.IsSuccess
            ? Result<EquipoDetail>.NotFound()
            : Result<EquipoDetail>.Success(MapListToDetail(equipo.Value));
    }

    public async Task<Result<List<EquipoList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<EquipoList>>.Success(result.Value)
            : Result<List<EquipoList>>.Error("Error al obtener equipos");
    }

    private static EquipoDetail MapListToDetail(EquipoList dto) => new()
    {
        Id = dto.Id,
        NombreGrupoEquipo = dto.NombreGrupoEquipo,
        Modelo = dto.Modelo,
        Marca = dto.Marca,
        CodigoImt = dto.CodigoImt,
        CodigoUcb = dto.CodigoUcb,
        NumeroSerial = dto.NumeroSerial,
        Ubicacion = dto.Ubicacion,
        EstadoEquipo = dto.EstadoEquipo,
        NombreGavetero = dto.NombreGavetero,
        CostoReferencia = dto.CostoReferencia,
        Descripcion = dto.Descripcion,
        TiempoMaximoPrestamo = dto.TiempoMaximoPrestamo,
        Procedencia = dto.Procedencia,
        EstadoEliminado = false
    };
}
