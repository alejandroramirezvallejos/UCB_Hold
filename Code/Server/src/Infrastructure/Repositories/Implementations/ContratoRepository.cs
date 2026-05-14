using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Infrastructure.Config;
using IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ContratoRepository : Repository<ContratoEntity, ContratoDto>
{
    public ContratoRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Result<ContratoEntity>> GetEntityByPrestamoId(int prestamoId)
    {
        var prestamo = await DbContext.Prestamos.AsNoTracking().FirstOrDefaultAsync(prestamo => prestamo.Id == prestamoId);

        if (prestamo == null || !prestamo.IdContrato.HasValue)
            return Result<ContratoEntity>.Error("Préstamo no encontrado o no tiene contrato");

        var contrato = await DbContext.Contratos.AsNoTracking()
            .FirstOrDefaultAsync(contrato => contrato.Id == prestamo.IdContrato.Value);

        if (contrato == null)
            return Result<ContratoEntity>.Error("Contrato no encontrado");

        return Result<ContratoEntity>.Success(contrato);
    }

    public async Task<Result<object>> DeleteByPrestamoId(int prestamoId)
    {
        var prestamo = await DbContext.Prestamos.FirstOrDefaultAsync(prestamo => prestamo.Id == prestamoId);

        if (prestamo == null || !prestamo.IdContrato.HasValue)
            return Result<object>.Error("Préstamo no encontrado o no tiene contrato");

        var contrato = await DbContext.Contratos.FirstOrDefaultAsync(contrato => contrato.Id == prestamo.IdContrato.Value);

        if (contrato == null)
            return Result<object>.Error("Contrato no encontrado");

        DbContext.Contratos.Remove(contrato);
        prestamo.IdContrato = null;
        DbContext.Prestamos.Update(prestamo);
        await DbContext.SaveChangesAsync();

        return Result<object>.Success(new { });
    }

    protected override ContratoDto MapToDto(ContratoEntity entity) => new()
    {
        Id = entity.Id,
        ContratoHtml = entity.ContratoHtml
    };
}
