using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService : Service<CarreraEntity, CarreraDetailDto, CarreraListDto>
{
    public CarreraService(CarreraRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<CarreraEntity> GetValidator() => new CarreraValidator();

    public new Result<CarreraDetailDto> Create(CarreraEntity entity) => base.Create(entity);

    public new Result<CarreraDetailDto> Update(CarreraEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<CarreraDetailDto> Get(int id) => base.Get(id);

    public new Result<List<CarreraListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
