using Ardalis.Result;
using IMT_Reservas.Server.Core.Entities;
using IMT_Reservas.Server.Infrastructure.MongoDb;
using MongoDB.Driver;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Implementations;

public class ContratoRepository
{
    private readonly MongoDbContexto _mongoContext;

    public ContratoRepository(MongoDbContexto mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public async Task<Result<Contrato>> Create(Contrato contrato)
    {
        try
        {
            await _mongoContext.GetContratos.InsertOneAsync(contrato);
            return Result<Contrato>.Success(contrato);
        }
        catch (Exception ex)
        {
            return Result<Contrato>.Error(ex.Message);
        }
    }

    public async Task<Result<Contrato>> GetByPrestamoId(int prestamoId)
    {
        try
        {
            var contrato = await _mongoContext.GetContratos
                .Find(c => c.PrestamoId == prestamoId && !c.EstadoEliminado)
                .FirstOrDefaultAsync();

            if (contrato == null)
                return Result<Contrato>.Error("Contrato no encontrado");

            return Result<Contrato>.Success(contrato);
        }
        catch (Exception ex)
        {
            return Result<Contrato>.Error(ex.Message);
        }
    }

    public async Task<Result<object>> Delete(int prestamoId)
    {
        try
        {
            var contrato = await _mongoContext.GetContratos
                .Find(c => c.PrestamoId == prestamoId && !c.EstadoEliminado)
                .FirstOrDefaultAsync();

            if (contrato == null)
                return Result<object>.Error("Contrato no encontrado");

            contrato.EstadoEliminado = true;
            await _mongoContext.GetContratos.ReplaceOneAsync(
                c => c.Id == contrato.Id,
                contrato
            );

            return Result<object>.Success(new { });
        }
        catch (Exception ex)
        {
            return Result<object>.Error(ex.Message);
        }
    }
}
