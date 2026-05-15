using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService : Service<CarreraEntity, CarreraRepository, CarreraDto>
{
    private readonly CarreraRepository _carreraRepository;
    private readonly CarreraMapper _mapper;
    private readonly IValidator<CarreraDto> _validator;

    public CarreraService(CarreraRepository repository, CarreraMapper mapper, IValidator<CarreraDto> validator)
        : base(repository) => (_carreraRepository, _mapper, _validator) = (repository, mapper, validator);

    public async Task<Result<CarreraDto>> Create(CarreraDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<CarreraDto>();

        var existing = await _carreraRepository.GetByNombre(dto.Nombre!);
        if (existing != null)
            return Result<CarreraDto>.Error($"Ya existe una carrera con nombre '{dto.Nombre}'");

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<CarreraDto>> Update(int id, CarreraDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<CarreraDto>();

        var existing = await _carreraRepository.GetByNombre(dto.Nombre!);
        if (existing != null && existing.Id != id)
            return Result<CarreraDto>.Error($"Ya existe otra carrera con nombre '{dto.Nombre}'");

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        return await base.Update(entity);
    }
}
