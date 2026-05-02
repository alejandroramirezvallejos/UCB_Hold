using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Dtos;
using IMT_Reservas.Server.Application.Features.GrupoEquipo.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using GrupoEquipoEntity = IMT_Reservas.Server.Core.Entities.GrupoEquipo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.GrupoEquipo;

public class GrupoEquipoService : Service<GrupoEquipoEntity, GrupoEquipoDetailDto, GrupoEquipoListDto>
{
    public GrupoEquipoService(GrupoEquipoRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<GrupoEquipoEntity> GetValidator() => new GrupoEquipoValidator();

    public new Result<GrupoEquipoDetailDto> Create(GrupoEquipoEntity entity) => base.Create(entity);

    public new Result<GrupoEquipoDetailDto> Update(GrupoEquipoEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<GrupoEquipoDetailDto> Get(int id) => base.Get(id);

    public new Result<List<GrupoEquipoListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
