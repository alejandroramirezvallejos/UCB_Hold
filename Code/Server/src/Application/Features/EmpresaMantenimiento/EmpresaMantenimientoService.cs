using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService : Service<EmpresaMantenimientoEntity, EmpresaMantenimientoDetailDto, EmpresaMantenimientoListDto>
{
    public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<EmpresaMantenimientoEntity> GetValidator() => new EmpresaMantenimientoValidator();

    public new Result<EmpresaMantenimientoDetailDto> Create(EmpresaMantenimientoEntity entity) => base.Create(entity);

    public new Result<EmpresaMantenimientoDetailDto> Update(EmpresaMantenimientoEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<EmpresaMantenimientoDetailDto> Get(int id) => base.Get(id);

    public new Result<List<EmpresaMantenimientoListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
