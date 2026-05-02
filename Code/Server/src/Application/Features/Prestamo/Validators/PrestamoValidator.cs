using Ardalis.Result;
using IMT_Reservas.Server.Core.Abstractions;
using IMT_Reservas.Server.Core.Errors;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;

namespace IMT_Reservas.Server.Application.Features.Prestamo.Validators;

public class PrestamoValidator : Validator<PrestamoEntity>
{
    public override Result<object> Validate(PrestamoEntity entity)
    {
        var validation = RequiredPositiveInt(entity.IdUsuario, nameof(entity.IdUsuario));
        if (!validation.IsSuccess) return validation;

        if (entity.FechaSolicitud == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaSolicitud)));

        if (entity.FechaInicio == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaInicio)));

        if (entity.FechaFin == default)
            return Result<object>.Invalid(ErrorFactory.RequiredField(nameof(entity.FechaFin)));

        if (entity.FechaFin <= entity.FechaInicio)
            return Result<object>.Invalid(new ValidationError(nameof(entity.FechaFin), "FechaFin must be after FechaInicio"));

        return Result<object>.Success(null);
    }
}
