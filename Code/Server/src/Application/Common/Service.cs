namespace IMT_Reservas.Server.Application.Common;

using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public abstract class Service<TEntity, TDetailDto, TListDto>
    where TEntity : Entity
    where TDetailDto : class
    where TListDto : class
{
    protected readonly IRepository<TListDto> Repository;
    protected readonly IMapper AutoMapper;

    protected Service(IRepository<TListDto> repository, IMapper autoMapper)
    {
        Repository = repository;
        AutoMapper = autoMapper;
    }

    protected abstract Validator<TEntity> GetValidator();

    protected Result<TDetailDto> Create(TEntity entity)
    {
        var validation = GetValidator().Validate(entity);
        if (!validation.IsSuccess)
        {
            var errors = validation.ValidationErrors.Select(e => e.ErrorMessage ?? e.Identifier).ToList();
            return Result<TDetailDto>.Error(string.Join("; ", errors));
        }

        var parameters = IMT_Reservas.Server.Application.Common.Mapper.ToParameters(entity);
        var result = Repository.Create(parameters);
        return result.IsSuccess
            ? Result<TDetailDto>.Success(AutoMapper.Map<TDetailDto>(result.Value))
            : Result<TDetailDto>.Error(string.Join(", ", result.Errors));
    }

    protected Result<TDetailDto> Update(TEntity entity)
    {
        var validation = GetValidator().Validate(entity);
        if (!validation.IsSuccess)
        {
            var errors = validation.ValidationErrors.Select(e => e.ErrorMessage ?? e.Identifier).ToList();
            return Result<TDetailDto>.Error(string.Join("; ", errors));
        }

        var parameters = IMT_Reservas.Server.Application.Common.Mapper.ToParameters(entity);
        var result = Repository.Update(parameters);
        if (!result.IsSuccess) return Result<TDetailDto>.Error(string.Join(", ", result.Errors));

        var updated = Repository.Get(entity.Id);
        return updated.IsSuccess
            ? Result<TDetailDto>.Success(AutoMapper.Map<TDetailDto>(updated.Value))
            : Result<TDetailDto>.NotFound();
    }

    protected Result<object> Delete(int id)
    {
        return Repository.Delete(id);
    }

    protected Result<TDetailDto> Get(int id)
    {
        var result = Repository.Get(id);
        return result.IsSuccess
            ? Result<TDetailDto>.Success(AutoMapper.Map<TDetailDto>(result.Value))
            : Result<TDetailDto>.NotFound();
    }

    protected Result<List<TListDto>> GetAll(QueryFilter? filter = null)
    {
        var result = Repository.GetAll(filter);
        return result.IsSuccess
            ? Result<List<TListDto>>.Success(result.Value)
            : Result<List<TListDto>>.Error(string.Join(", ", result.Errors));
    }
}
