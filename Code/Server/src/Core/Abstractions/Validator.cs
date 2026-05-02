namespace IMT_Reservas.Server.Core.Abstractions;

using Ardalis.Result;
using IMT_Reservas.Server.Core.Errors;

public abstract class Validator<TEntity> where TEntity : class
{
    private const int DefaultMaxLength = 255;
    private const int DefaultMaxTextLength = 1000;

    protected Result<object> RequiredString(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value?.Trim()))
            return Result<object>.Invalid(ErrorFactory.RequiredField(fieldName));
        return Result<object>.Success(null);
    }

    protected Result<object> MaxLength(string? value, string fieldName, int maxLength = DefaultMaxLength)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            return Result<object>.Invalid(new ValidationError(fieldName, $"Máximo {maxLength} caracteres"));
        return Result<object>.Success(null);
    }

    protected Result<object> ValidEmail(string? email)
    {
        if (email?.Contains("@") != true)
            return Result<object>.Invalid(new ValidationError(nameof(email), "Email no válido"));
        return Result<object>.Success(null);
    }

    protected Result<object> MinLength(string? value, string fieldName, int minLength)
    {
        if (!string.IsNullOrEmpty(value) && value.Length < minLength)
            return Result<object>.Invalid(new ValidationError(fieldName, $"Mínimo {minLength} caracteres"));
        return Result<object>.Success(null);
    }

    protected Result<object> NumericRange(int? value, string fieldName, int min, int max)
    {
        if (value.HasValue && (value < min || value > max))
            return Result<object>.Invalid(new ValidationError(fieldName, $"Debe estar entre {min} y {max}"));
        return Result<object>.Success(null);
    }

    protected Result<object> RequiredPositiveInt(int? value, string fieldName)
    {
        if (!value.HasValue || value <= 0)
            return Result<object>.Invalid(ErrorFactory.InvalidField<TEntity>(fieldName));
        return Result<object>.Success(null);
    }

    public abstract Result<object> Validate(TEntity entity);
}
