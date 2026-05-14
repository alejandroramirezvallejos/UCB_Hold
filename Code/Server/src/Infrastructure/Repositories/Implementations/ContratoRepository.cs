using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;
namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ContratoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ContratoRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Contrato>> Create(Contrato contrato)
    {
        _dbContext.Contratos.Add(contrato);
        await _dbContext.SaveChangesAsync();

        return Result<Contrato>.Success(contrato);
    }

    public async Task<Result<Contrato>> GetByPrestamoId(int prestamoId)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == prestamoId);
        
        if (prestamo == null || !prestamo.IdContrato.HasValue)
            return Result<Contrato>.Error("Préstamo no encontrado o no tiene contrato");

        var contrato = await _dbContext.Contratos
            .FirstOrDefaultAsync(c => c.Id == prestamo.IdContrato.Value);

        if (contrato == null)
            return Result<Contrato>.Error("Contrato no encontrado");

        return Result<Contrato>.Success(contrato);
    }

    public async Task<Result<object>> Delete(int prestamoId)
    {
        var prestamo = await _dbContext.Prestamos.FirstOrDefaultAsync(p => p.Id == prestamoId);
        
        if (prestamo == null || !prestamo.IdContrato.HasValue)
            return Result<object>.Error("Préstamo no encontrado o no tiene contrato");

        var contrato = await _dbContext.Contratos
            .FirstOrDefaultAsync(c => c.Id == prestamo.IdContrato.Value);

        if (contrato == null)
            return Result<object>.Error("Contrato no encontrado");

        _dbContext.Contratos.Remove(contrato);
        prestamo.IdContrato = null;
        _dbContext.Prestamos.Update(prestamo);
        await _dbContext.SaveChangesAsync();

        return Result<object>.Success(new { });
    }
}
