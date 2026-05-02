using Ardalis.Result;
using AutoMapper;
using IMT_Reservas.Server.Application.Features.Notificacion.Dtos;
using IMT_Reservas.Server.Core.Abstractions;
using NotificacionEntity = IMT_Reservas.Server.Core.Entities.Notificacion;
using IMT_Reservas.Server.Infrastructure.Repositories;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

namespace IMT_Reservas.Server.Application.Features.Notificacion;

public class NotificacionService
{
    private readonly NotificacionRepository _repository;
    private readonly IMapper _mapper;

    public NotificacionService(NotificacionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<NotificacionDetailDto>> Create(NotificacionEntity entity)
    {
        var parameters = Common.Mapper.ToParameters(entity);
        var result = await _repository.CreateAsync(parameters);
        return result.IsSuccess
            ? Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(result.Value))
            : Result<NotificacionDetailDto>.Error(string.Join(", ", result.Errors));
    }

    public async Task<Result<NotificacionDetailDto>> Update(NotificacionEntity entity)
    {
        var parameters = Common.Mapper.ToParameters(entity);
        var result = await _repository.UpdateAsync(parameters);
        if (!result.IsSuccess) return Result<NotificacionDetailDto>.Error(string.Join(", ", result.Errors));

        var updated = await _repository.GetByIdAsync(entity.Id);
        return updated.IsSuccess
            ? Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(updated.Value))
            : Result<NotificacionDetailDto>.NotFound();
    }

    public async Task<Result<object>> Delete(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<Result<NotificacionDetailDto>> Get(int id)
    {
        var result = await _repository.GetByIdAsync(id);
        return result.IsSuccess
            ? Result<NotificacionDetailDto>.Success(_mapper.Map<NotificacionDetailDto>(result.Value))
            : Result<NotificacionDetailDto>.NotFound();
    }

    public async Task<Result<List<NotificacionListDto>>> GetAll(QueryFilter? filter = null)
    {
        var result = await _repository.GetAllAsync(filter);
        return result.IsSuccess
            ? Result<List<NotificacionListDto>>.Success(result.Value)
            : Result<List<NotificacionListDto>>.Error(string.Join(", ", result.Errors));
    }
}
