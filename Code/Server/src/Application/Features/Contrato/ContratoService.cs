using Ardalis.Result;
using IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using IMT_Reservas.Server.Application.Features.Archivo;
namespace IMT_Reservas.Server.Application.Features.Contrato;

public class ContratoService
{
    private readonly ArchivoService _archivoService;
    private readonly ContratoRepository _contratoRepository;
    private readonly PrestamoRepository _prestamoRepository;

    public ContratoService(ArchivoService archivoService, ContratoRepository contratoRepository, PrestamoRepository prestamoRepository)
    {
        _archivoService = archivoService;
        _contratoRepository = contratoRepository;
        _prestamoRepository = prestamoRepository;
    }

    public async Task<Result<ContratoResponse>> Create(int prestamoId, Stream fileStream, string filename)
    {
        var prestamoExiste = await _prestamoRepository.ExisteActivoPorId(prestamoId);
        
        if (!prestamoExiste)
            return Result<ContratoResponse>.Error("Préstamo no encontrado");

        var uploadResultado = await _archivoService.Upload(fileStream, filename);
        
        if (!uploadResultado.IsSuccess)
            return Result<ContratoResponse>.Error(uploadResultado.Errors.FirstOrDefault() ?? "Error al subir archivo");

        var contrato = new Core.Entities.Contrato
        {
            PrestamoId = prestamoId,
            FileId = uploadResultado.Value,
            FechaCreacion = DateTime.UtcNow,
            EstadoEliminado = false
        };

        var crearResultado = await _contratoRepository.Create(contrato);
        
        if (!crearResultado.IsSuccess)
        {
            await _archivoService.Delete(uploadResultado.Value);
            return Result<ContratoResponse>.Error(crearResultado.Errors.FirstOrDefault() ?? "Error al crear contrato");
        }

        var respuesta = new ContratoResponse
        {
            Id = crearResultado.Value.Id.ToString(),
            PrestamoId = crearResultado.Value.PrestamoId,
            FileId = crearResultado.Value.FileId,
            FechaCreacion = crearResultado.Value.FechaCreacion
        };

        return Result<ContratoResponse>.Success(respuesta);
    }

    public async Task<Result<ContratoResponse>> Get(int prestamoId)
    {
        var obtenerResultado = await _contratoRepository.GetByPrestamoId(prestamoId);
        
        if (!obtenerResultado.IsSuccess)
            return Result<ContratoResponse>.Error(obtenerResultado.Errors.FirstOrDefault() ?? "Contrato no encontrado");

        var respuesta = new ContratoResponse
        {
            Id = obtenerResultado.Value.Id.ToString(),
            PrestamoId = obtenerResultado.Value.PrestamoId,
            FileId = obtenerResultado.Value.FileId,
            FechaCreacion = obtenerResultado.Value.FechaCreacion
        };

        return Result<ContratoResponse>.Success(respuesta);
    }

    public async Task<Result<object>> Delete(int prestamoId)
    {
        var obtenerResultado = await _contratoRepository.GetByPrestamoId(prestamoId);
        
        if (obtenerResultado.IsSuccess && obtenerResultado.Value.FileId != null)
            await _archivoService.Delete(obtenerResultado.Value.FileId);

        var eliminarResultado = await _contratoRepository.Delete(prestamoId);
        
        if (!eliminarResultado.IsSuccess)
            return Result<object>.Error(eliminarResultado.Errors.FirstOrDefault() ?? "Error al eliminar contrato");

        return Result<object>.Success(new { });
    }
}
