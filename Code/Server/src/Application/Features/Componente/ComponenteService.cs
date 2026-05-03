using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService
{
    private readonly ComponenteRepository _repository;

    public ComponenteService(ComponenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ComponenteDto>> Create(ComponenteEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<ComponenteDto>> Update(ComponenteEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<ComponenteDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<ComponenteDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
