using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Abstractions;
namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService
{
    private readonly AccesorioRepository _repository;

    public AccesorioService(AccesorioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AccesorioDetailDto>> Create(AccesorioEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<AccesorioDetailDto>.Error("Error al crear accesorio")
            : Result<AccesorioDetailDto>.Created(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<AccesorioDetailDto>> Update(AccesorioEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<AccesorioDetailDto>.Error("Error al actualizar accesorio")
            : Result<AccesorioDetailDto>.Success(MapListDtoToDetailDto(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar accesorio");
    }

    public async Task<Result<AccesorioDetailDto>> Get(int id)
    {
        var accesorio = await _repository.Get(id);
        
        return !accesorio.IsSuccess
            ? Result<AccesorioDetailDto>.NotFound()
            : Result<AccesorioDetailDto>.Success(MapListDtoToDetailDto(accesorio.Value));
    }

    public async Task<Result<List<AccesorioListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<AccesorioListDto>>.Success(result.Value)
            : Result<List<AccesorioListDto>>.Error("Error al obtener accesorios");
    }

    private static AccesorioDetailDto MapListDtoToDetailDto(AccesorioListDto dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Descripcion = dto.Descripcion,
        Modelo = dto.Modelo,
        UrlDataSheet = dto.UrlDataSheet,
        Precio = dto.Precio,
        IdEquipo = 0,
        Tipo = dto.Tipo,
        EstadoEliminado = false
    };
}
