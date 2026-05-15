using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;

public class EmpresaMantenimientoValidator : AbstractValidator<EmpresaMantenimientoDto>
{
    public EmpresaMantenimientoValidator()
        => RuleFor(empresa => empresa.NombreEmpresa).NotEmpty().WithMessage("NombreEmpresa requerido");
}
