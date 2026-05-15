using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoService : Service<EmpresaMantenimientoEntity, EmpresaMantenimientoRepository, EmpresaMantenimientoDto>
{
    private readonly EmpresaMantenimientoMapper _mapper;

    public EmpresaMantenimientoService(EmpresaMantenimientoRepository repository, EmpresaMantenimientoMapper mapper,
        IValidator<EmpresaMantenimientoDto> validator)
        : base(repository, validator)
    {
        _mapper = mapper;
    }

    protected override EmpresaMantenimientoEntity MapToEntity(EmpresaMantenimientoDto dto) => _mapper.ToEntity(dto);

    public Task<Result<EmpresaMantenimientoDto>> Create(EmpresaMantenimientoDto dto) => ValidateAndCreate(dto);

    public Task<Result<EmpresaMantenimientoDto>> Update(int id, EmpresaMantenimientoDto dto) => ValidateAndUpdate(id, dto);
}
