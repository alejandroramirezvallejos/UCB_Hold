using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.PostgreSQL;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoService
{
    private readonly ContratoRepository _repository;
    private readonly ApplicationDbContext _postgresContext;

    public ContratoService(ContratoRepository repository, ApplicationDbContext postgresContext)
    {
        _repository = repository;
        _postgresContext = postgresContext;
    }

    public async Task<Result<ContratoDto>> Create(int? prestamoId, string contenidoHtml)
    {
        if (string.IsNullOrEmpty(contenidoHtml))
            return Result<ContratoDto>.Error("Contenido de contrato requerido");

        var prestamo = await _postgresContext.Prestamos.FindAsync(prestamoId);

        if (prestamo == null)
            return Result<ContratoDto>.Error("Préstamo no existe");

        if (!string.IsNullOrEmpty(prestamo.IdContrato))
            return Result<ContratoDto>.Error("Contrato ya existe para este préstamo");

        var contrato = new ContratoEntity
        {
            MongoId = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            PrestamoId = prestamoId.Value,
            ContenidoBase64 = contenidoHtml,
            FechaCreacion = DateTime.UtcNow,
            EstadoEliminado = false
        };

        var resultado = await _repository.Create(contrato);

        if (!resultado.IsSuccess)
            return Result<ContratoDto>.Error(resultado.Errors.FirstOrDefault() ?? "Error crear contrato");

        prestamo.IdContrato = resultado.Value.MongoId;
        _postgresContext.Prestamos.Update(prestamo);
        await _postgresContext.SaveChangesAsync();

        return Result<ContratoDto>.Success(new ContratoDto
        {
            PrestamoId = prestamoId,
            FechaCreacion = contrato.FechaCreacion,
            ContenidoBase64 = contenidoHtml
        });
    }

    public async Task<Result<ContratoDto>> GetByPrestamoId(int prestamoId)
    {
        var resultado = await _repository.GetByPrestamoId(prestamoId);

        if (!resultado.IsSuccess)
            return Result<ContratoDto>.Error("Contrato no encontrado");

        return Result<ContratoDto>.Success(new ContratoDto
        {
            Id = resultado.Value.Id,
            PrestamoId = resultado.Value.PrestamoId,
            FechaCreacion = resultado.Value.FechaCreacion,
            ContenidoBase64 = resultado.Value.ContenidoBase64
        });
    }

    public async Task<Result<object>> Delete(int prestamoId) => await _repository.Delete(prestamoId);
}
