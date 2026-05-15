using Ardalis.Result;
using FluentValidation.Results;
namespace IMT_Reservas.Server.Application.Abstraction;

public static class Validation
{
    public static Result<T> ToResult<T>(this ValidationResult validation) where T : class
        => validation.IsValid
            ? Result<T>.Success(null!)
            : Result<T>.Error(string.Join(" | ", validation.Errors.Select(error => error.ErrorMessage)));
}
