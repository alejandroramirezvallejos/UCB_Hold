using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using IMT_Reservas.Server.Application.Features.Prestamo.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Prestamo;

public class PrestamoService : Service<PrestamoEntity, PrestamoDetailDto, PrestamoListDto>
{
    public PrestamoService(PrestamoRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<PrestamoEntity> GetValidator() => new PrestamoValidator();

    public new Result<PrestamoDetailDto> Create(PrestamoEntity entity) => base.Create(entity);

    public new Result<PrestamoDetailDto> Update(PrestamoEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<PrestamoDetailDto> Get(int id) => base.Get(id);

    public new Result<List<PrestamoListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
