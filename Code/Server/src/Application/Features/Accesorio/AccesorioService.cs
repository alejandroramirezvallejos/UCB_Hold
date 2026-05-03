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

    public async Task<Result<AccesorioDetail>> Create(AccesorioEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return !result.IsSuccess
            ? Result<AccesorioDetail>.Error("Error al crear accesorio")
            : Result<AccesorioDetail>.Created(MapListToDetail(result.Value));
    }

    public async Task<Result<AccesorioDetail>> Update(AccesorioEntity entity)
    {
        var result = await _repository.Update(entity);
        
        return !result.IsSuccess
            ? Result<AccesorioDetail>.Error("Error al actualizar accesorio")
            : Result<AccesorioDetail>.Success(MapListToDetail(result.Value));
    }

    public async Task<Result<object>> Delete(int id)
    {
        var result = await _repository.Delete(id);
        
        return result.IsSuccess
            ? Result<object>.Success(null!)
            : Result<object>.Error("Error al eliminar accesorio");
    }

    public async Task<Result<AccesorioDetail>> Get(int id)
    {
        var accesorio = await _repository.Get(id);
        
        return !accesorio.IsSuccess
            ? Result<AccesorioDetail>.NotFound()
            : Result<AccesorioDetail>.Success(MapListToDetail(accesorio.Value));
    }

    public async Task<Result<List<AccesorioList>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAll(filter);
        
        return result.IsSuccess
            ? Result<List<AccesorioList>>.Success(result.Value)
            : Result<List<AccesorioList>>.Error("Error al obtener accesorios");
    }

    private static AccesorioDetail MapListToDetail(AccesorioList dto) => new()
    {
        Id = dto.Id,
        Nombre = dto.Nombre,
        Modelo = dto.Modelo,
        Tipo = dto.Tipo,
        CodigoImt = null,
        Descripcion = dto.Descripcion,
        Precio = dto.Precio,
        UrlDataSheet = dto.UrlDataSheet,
        EstadoEliminado = false
    };
}
