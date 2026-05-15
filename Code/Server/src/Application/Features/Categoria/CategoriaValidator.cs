using FluentValidation;
namespace IMT_Reservas.Server.Application.Features.Categoria;

public class CategoriaValidator : AbstractValidator<CategoriaDto>
{
    public CategoriaValidator()
        => RuleFor(categoria => categoria.Nombre).NotEmpty().WithMessage("Nombre requerido");
}
