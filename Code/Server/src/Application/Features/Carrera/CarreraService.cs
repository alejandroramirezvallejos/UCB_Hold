using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService : Service<CarreraEntity, CarreraRepository, CarreraDto>
{
    private readonly CarreraMapper _mapper;

    public CarreraService(CarreraRepository repository, CarreraMapper mapper, IValidator<CarreraDto> validator)
        : base(repository, validator) { _mapper = mapper; }

    protected override CarreraEntity MapToEntity(CarreraDto dto) => _mapper.ToEntity(dto);

    public async Task<Result<CarreraDto>> Create(CarreraDto dto)
    {
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<CarreraDto>();

        return await base.Create(MapToEntity(dto));
    }

    public async Task<Result<CarreraDto>> Update(int id, CarreraDto dto)
    {
        dto.Id = id;
        var validation = await Validator.ValidateAsync(dto);
        
        if (!validation.IsValid) 
            return validation.ToResult<CarreraDto>();

        var entity = MapToEntity(dto);
        entity.Id = id;
        
        return await base.Update(entity);
    }
}
