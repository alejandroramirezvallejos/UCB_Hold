using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService : Service<EmpresaMantenimientoEntity, EmpresaMantenimientoRepository, EmpresaMantenimientoDto>
{
    private readonly EmpresaMantenimientoMapper _mapper;
    private readonly IValidator<EmpresaMantenimientoDto> _validator;

    public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository, EmpresaMantenimientoMapper mapper, IValidator<EmpresaMantenimientoDto> validator)
        : base(repository) => (_mapper, _validator) = (mapper, validator);

    public async Task<Result<EmpresaMantenimientoDto>> Create(EmpresaMantenimientoDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<EmpresaMantenimientoDto>();

        return await base.Create(_mapper.ToEntity(dto));
    }

    public async Task<Result<EmpresaMantenimientoDto>> Update(int id, EmpresaMantenimientoDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<EmpresaMantenimientoDto>();

        var entity = _mapper.ToEntity(dto);
        entity.Id = id;
        return await base.Update(entity);
    }
}
