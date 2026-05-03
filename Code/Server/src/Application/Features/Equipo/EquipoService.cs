using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService
{
    private readonly EquipoRepository _repository;

    public EquipoService(EquipoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<EquipoDto>> Create(EquipoEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<EquipoDto>> Update(EquipoEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<EquipoDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<EquipoDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
