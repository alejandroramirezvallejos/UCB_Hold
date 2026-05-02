using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using IMT_Reservas.Server.Application.Features.Gavetero.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Gavetero;

public class GaveteroService : Service<GaveteroEntity, GaveteroDetailDto, GaveteroListDto>
{
    public GaveteroService(GaveteroRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<GaveteroEntity> GetValidator() => new GaveteroValidator();

    public new Result<GaveteroDetailDto> Create(GaveteroEntity entity) => base.Create(entity);

    public new Result<GaveteroDetailDto> Update(GaveteroEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<GaveteroDetailDto> Get(int id) => base.Get(id);

    public new Result<List<GaveteroListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
