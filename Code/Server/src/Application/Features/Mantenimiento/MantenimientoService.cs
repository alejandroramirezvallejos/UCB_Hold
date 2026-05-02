using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Mantenimiento;

public class MantenimientoService : Service<MantenimientoEntity, MantenimientoDetailDto, MantenimientoListDto>
{
    public MantenimientoService(MantenimientoRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<MantenimientoEntity> GetValidator() => new MantenimientoValidator();

    public new Result<MantenimientoDetailDto> Create(MantenimientoEntity entity) => base.Create(entity);

    public new Result<MantenimientoDetailDto> Update(MantenimientoEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<MantenimientoDetailDto> Get(int id) => base.Get(id);

    public new Result<List<MantenimientoListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
