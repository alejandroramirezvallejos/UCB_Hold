using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraService : Service<CarreraEntity, CarreraRepository, CarreraDto>
{
    private readonly CarreraRepository _carreraRepository;

    public CarreraService(CarreraRepository repository) : base(repository)
    {
        _carreraRepository = repository;
    }

    public override async Task<Result<CarreraDto>> Create(CarreraEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            return Result<CarreraDto>.Error("Nombre de carrera es requerido");

        var existing = await _carreraRepository.GetByNombre(entity.Nombre);
        
        if (existing != null)
            return Result<CarreraDto>.Error($"Ya existe una carrera con nombre '{entity.Nombre}'");

        return await base.Create(entity);
    }

    public override async Task<Result<CarreraDto>> Update(CarreraEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            return Result<CarreraDto>.Error("Nombre de carrera es requerido");

        var existing = await _carreraRepository.GetByNombre(entity.Nombre);
        
        if (existing != null && existing.Id != entity.Id)
            return Result<CarreraDto>.Error($"Ya existe otra carrera con nombre '{entity.Nombre}'");

        return await base.Update(entity);
    }
}
