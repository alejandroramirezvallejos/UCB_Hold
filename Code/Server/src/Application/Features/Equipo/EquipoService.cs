using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Equipo.Dtos;
using IMT_Reservas.Server.Application.Features.Equipo.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using EquipoEntity = IMT_Reservas.Server.Core.Entities.Equipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Equipo;

public class EquipoService : Service<EquipoEntity, EquipoDetailDto, EquipoListDto>
{
    public EquipoService(EquipoRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<EquipoEntity> GetValidator() => new EquipoValidator();

    public new Result<EquipoDetailDto> Create(EquipoEntity entity) => base.Create(entity);

    public new Result<EquipoDetailDto> Update(EquipoEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<EquipoDetailDto> Get(int id) => base.Get(id);

    public new Result<List<EquipoListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
