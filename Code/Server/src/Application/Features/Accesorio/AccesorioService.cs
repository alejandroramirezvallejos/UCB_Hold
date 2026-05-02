using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using IMT_Reservas.Server.Application.Features.Accesorio.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Accesorio;

public class AccesorioService : Service<AccesorioEntity, AccesorioDetailDto, AccesorioListDto>
{
    public AccesorioService(AccesorioRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<AccesorioEntity> GetValidator() => new AccesorioValidator();

    public new Result<AccesorioDetailDto> Create(AccesorioEntity entity) => base.Create(entity);

    public new Result<AccesorioDetailDto> Update(AccesorioEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<AccesorioDetailDto> Get(int id) => base.Get(id);

    public new Result<List<AccesorioListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
