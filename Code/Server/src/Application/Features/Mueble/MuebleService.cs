using Ardalis.Result;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Application.Features.Mueble;

public class MuebleService : Service<MuebleEntity, MuebleRepository, MuebleDto>
{
    public MuebleService(MuebleRepository repository) : base(repository) { }

    public override async Task<Result<MuebleDto>> Create(MuebleEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nombre))
            return Result<MuebleDto>.Error("Nombre mueble requerido");

        if (entity.Costo <= 0)
            return Result<MuebleDto>.Error("Costo debe ser mayor a 0");

        return await base.Create(entity);
    }
}
