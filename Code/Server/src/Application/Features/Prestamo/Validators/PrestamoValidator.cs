using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Core.Errors;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;

namespace IMT_Reservas.Server.Application.Features.Prestamo.Validators;

public class PrestamoValidator : Validator<PrestamoEntity>
{
    public override Result<object> Validate(PrestamoEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Carnet))
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.Carnet)));

        if (entity.FechaSolicitud == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaSolicitud)));

        if (entity.FechaPrestamoEsperada == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaPrestamoEsperada)));

        if (entity.FechaDevolucionEsperada == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaDevolucionEsperada)));

        if (entity.FechaDevolucionEsperada <= entity.FechaPrestamoEsperada)
            return Result<object>.Invalid(new ValidationError(nameof(entity.FechaDevolucionEsperada), "La fecha de devolución debe ser posterior a la fecha de préstamo"));

        return Result<object>.Success(null);
    }
}
