using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoService : Service<ContratoEntity, ContratoRepository, ContratoDto>
{
    public ContratoService(ContratoRepository repository, ContratoMapper mapper, IValidator<ContratoDto> validator)
        : base(repository, validator, mapper) { }

    public async Task<Result<ContratoDto>> CreateForPrestamo(int prestamoId, string contenidoHtml)
    {
        var dto = new ContratoDto { ContratoHtml = contenidoHtml, PrestamoId = prestamoId };
        var validation = await Validator.ValidateAsync(dto);

        if (!validation.IsValid)
            return validation.ToResult<ContratoDto>();

        var prestamo = await Repository.FindPrestamoById(prestamoId);

        if (prestamo == null)
            return Result<ContratoDto>.Error("Préstamo no existe");

        if (prestamo.IdContrato.HasValue)
            return Result<ContratoDto>.Error("Contrato ya existe para este préstamo");

        var contrato = MapToEntity(dto);
        var result = await Repository.Create(contrato);

        if (!result.IsSuccess)
            return Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "Error al crear contrato");

        prestamo.IdContrato = result.Value.Id;
        await Repository.SavePrestamo(prestamo);

        return result;
    }

    public async Task<Result<ContratoDto>> GetByPrestamoId(int prestamoId)
    {
        var result = await Repository.GetEntityByPrestamoId(prestamoId);

        if (!result.IsSuccess)
            return Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "Contrato no encontrado");

        return Result<ContratoDto>.Success(MapToDto(result.Value));
    }
}
