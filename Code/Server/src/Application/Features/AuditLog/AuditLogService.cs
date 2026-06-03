using Ardalis.Result;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using AuditLogEntity = IMT_Reservas.Server.Core.Entities.AuditLog;
namespace IMT_Reservas.Server.Application.Features.AuditLog;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(AuditLogRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository          = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Log(AuditAccion accion, string entidad, string? entidadId, string? detalle = null)
    {
        var adminCarnet = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "sistema";
        var adminNombre = _httpContextAccessor.HttpContext?.User.FindFirst("nombre")?.Value ?? string.Empty;

        await _repository.WriteLog(new AuditLogEntity
        {
            AdminCarnet = adminCarnet,
            AdminNombre = adminNombre,
            Accion      = accion.ToString(),
            Entidad     = entidad,
            EntidadId   = entidadId,
            Detalle     = detalle,
            Timestamp   = DateTime.UtcNow
        });
    }

    public async Task<Result<List<AuditLogDto>>> GetFiltered(
        string? entidad,
        string? carnet,
        DateTime? desde,
        DateTime? hasta)
    {
        var logs = await _repository.GetFiltered(entidad, carnet, desde, hasta);
        
        return Result<List<AuditLogDto>>.Success(logs);
    }
}
