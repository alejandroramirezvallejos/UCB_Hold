using Ardalis.Result;
using FluentValidation;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoService : Service<ContratoEntity, ContratoRepository, ContratoDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ContratoMapper _mapper;
    private readonly IValidator<ContratoDto> _validator;

    public ContratoService(ContratoRepository repository, ApplicationDbContext dbContext, ContratoMapper mapper, IValidator<ContratoDto> validator)
        : base(repository) => (_dbContext, _mapper, _validator) = (dbContext, mapper, validator);

    public async Task<Result<ContratoDto>> CreateForPrestamo(int prestamoId, string contenidoHtml)
    {
        var dto = new ContratoDto { ContratoHtml = contenidoHtml, PrestamoId = prestamoId };
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid) return validation.ToResult<ContratoDto>();

        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == prestamoId);
        if (prestamo == null) return Result<ContratoDto>.Error("Préstamo no existe");
        if (prestamo.IdContrato.HasValue) return Result<ContratoDto>.Error("Contrato ya existe para este préstamo");

        var contrato = new ContratoEntity { ContratoHtml = contenidoHtml };
        var result = await Repository.Create(contrato);
        if (!result.IsSuccess)
            return Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "Error al crear contrato");

        prestamo.IdContrato = result.Value.Id;
        _dbContext.Prestamos.Update(prestamo);
        await _dbContext.SaveChangesAsync();

        return result;
    }

    public async Task<Result<ContratoDto>> GetByPrestamoId(int prestamoId)
    {
        var result = await Repository.GetEntityByPrestamoId(prestamoId);
        if (!result.IsSuccess)
            return Result<ContratoDto>.Error(result.Errors.FirstOrDefault() ?? "Contrato no encontrado");

        return Result<ContratoDto>.Success(_mapper.ToDto(result.Value));
    }

    public async Task<Result<object>> DeleteByPrestamoId(int prestamoId)
        => await Repository.DeleteByPrestamoId(prestamoId);
}
