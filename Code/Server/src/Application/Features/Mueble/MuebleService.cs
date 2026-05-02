using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Mueble.Dtos;
using IMT_Reservas.Server.Application.Features.Mueble.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService : Service<MuebleEntity, MuebleDetailDto, MuebleListDto>
{
    public MuebleService(MuebleRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<MuebleEntity> GetValidator() => new MuebleValidator();

    public new Result<MuebleDetailDto> Create(MuebleEntity entity) => base.Create(entity);

    public new Result<MuebleDetailDto> Update(MuebleEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<MuebleDetailDto> Get(int id) => base.Get(id);

    public new Result<List<MuebleListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
