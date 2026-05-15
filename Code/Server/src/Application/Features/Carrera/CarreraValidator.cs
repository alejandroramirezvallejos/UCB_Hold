using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Carrera;

public class CarreraValidator : AbstractValidator<CarreraDto>
{
    public CarreraValidator()
        => RuleFor(carrera => carrera.Nombre).NotEmpty().WithMessage("Nombre requerido");
}
