using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Componente.Dtos;
using IMT_Reservas.Server.Application.Features.Componente.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Componente;

public class ComponenteService : Service<ComponenteEntity, ComponenteDetailDto, ComponenteListDto>
{
    public ComponenteService(ComponenteRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<ComponenteEntity> GetValidator() => new ComponenteValidator();

    public new Result<ComponenteDetailDto> Create(ComponenteEntity entity) => base.Create(entity);

    public new Result<ComponenteDetailDto> Update(ComponenteEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<ComponenteDetailDto> Get(int id) => base.Get(id);

    public new Result<List<ComponenteListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
