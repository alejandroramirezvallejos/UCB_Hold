using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using IMT_Reservas.Server.Application.Features.Categoria.Validators;
using IMT_Reservas.Server.Core.Abstractions;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaService : Service<CategoriaEntity, CategoriaDetailDto, CategoriaListDto>
{
    public CategoriaService(CategoriaRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }

    protected override Validator<CategoriaEntity> GetValidator() => new CategoriaValidator();

    public new Result<CategoriaDetailDto> Create(CategoriaEntity entity) => base.Create(entity);

    public new Result<CategoriaDetailDto> Update(CategoriaEntity entity) => base.Update(entity);

    public new Result<object> Delete(int id) => base.Delete(id);

    public new Result<CategoriaDetailDto> Get(int id) => base.Get(id);

    public new Result<List<CategoriaListDto>> GetAll(QueryFilter? filter = null) => base.GetAll(filter);
}
