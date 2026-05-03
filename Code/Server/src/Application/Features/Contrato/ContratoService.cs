using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoService
{
    private readonly ContratoRepository _repository;

    public ContratoService(ContratoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ContratoDto>> Create(ContratoEntity entity)
    {
        var result = await _repository.Create(entity);
        
        return result.IsSuccess
            ? Result<ContratoDto>.Success(new ContratoDto { Id = result.Value.Id })
            : Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "Error al crear");
    }

    public async Task<Result<object>> Delete(int prestamoId)
        => await _repository.Delete(prestamoId);

    public async Task<Result<ContratoDto>> GetByPrestamoId(int prestamoId)
    {
        var result = await _repository.GetByPrestamoId(prestamoId);
        return result.IsSuccess
            ? Result<ContratoDto>.Success(new ContratoDto { Id = result.Value.Id })
            : Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "No encontrado");
    }
}
