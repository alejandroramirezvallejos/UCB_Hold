using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Core.Abstractions;
using ComentarioEntity = IMT_Reservas.Server.Core.Entities.Comentario;
using IMT_Reservas.Server.Application.Features.Comentario.Dtos;
using IMT_Reservas.Server.Application.Features.Comentario.Validators;
using IMT_Reservas.Server.Infrastructure.Repositories;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Comentario;

public class ComentarioService
{
    private readonly ComentarioRepository _repository;
    private readonly IMapper _mapper;

    public ComentarioService(ComentarioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ComentarioDetailDto>> Create(ComentarioEntity entity)
    {
        var parameters = Common.Mapper.ToParameters(entity);
        var result = await _repository.CreateAsync(parameters);
        return result.IsSuccess
            ? Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(result.Value))
            : Result<ComentarioDetailDto>.Error(string.Join(", ", result.Errors));
    }

    public async Task<Result<ComentarioDetailDto>> Update(ComentarioEntity entity)
    {
        var parameters = Common.Mapper.ToParameters(entity);
        var result = await _repository.UpdateAsync(parameters);
        if (!result.IsSuccess) return Result<ComentarioDetailDto>.Error(string.Join(", ", result.Errors));

        var updated = await _repository.GetByIdAsync(entity.Id);
        return updated.IsSuccess
            ? Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(updated.Value))
            : Result<ComentarioDetailDto>.NotFound();
    }

    public async Task<Result<object>> Delete(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<Result<ComentarioDetailDto>> Get(int id)
    {
        var result = await _repository.GetByIdAsync(id);
        return result.IsSuccess
            ? Result<ComentarioDetailDto>.Success(_mapper.Map<ComentarioDetailDto>(result.Value))
            : Result<ComentarioDetailDto>.NotFound();
    }

    public async Task<Result<List<ComentarioListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAllAsync(filter);
        return result.IsSuccess
            ? Result<List<ComentarioListDto>>.Success(result.Value)
            : Result<List<ComentarioListDto>>.Error(string.Join(", ", result.Errors));
    }
}
