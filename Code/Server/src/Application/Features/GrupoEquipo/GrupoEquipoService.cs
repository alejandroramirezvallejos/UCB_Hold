using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Core.Common;
namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService
{
    private readonly GrupoEquipoRepository _repository;

    public GrupoEquipoService(GrupoEquipoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GrupoEquipoDto>> Create(GrupoEquipoEntity entity)
        => await _repository.Create(entity);

    public async Task<Result<GrupoEquipoDto>> Update(GrupoEquipoEntity entity)
        => await _repository.Update(entity);

    public async Task<Result<object>> Delete(int id)
        => await _repository.Delete(id);

    public async Task<Result<GrupoEquipoDto>> Get(int id)
        => await _repository.Get(id);

    public async Task<Result<List<GrupoEquipoDto>>> GetAll(QueryFilter? filter = null)
        => await _repository.GetAll(filter);
}
