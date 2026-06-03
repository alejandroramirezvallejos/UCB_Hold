using IMT_Reservas.Server.Application.Features.AuditLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Authorize(Roles = "administrador")]
[Route("api/[controller]")]
[ApiController]
public class AuditLogController : ControllerBase
{
    private readonly AuditLogService _service;

    public AuditLogController(AuditLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? entidad,
        [FromQuery] string? carnet,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta)
    {
        var result = await _service.GetFiltered(entidad, carnet, desde, hasta);
       
        return Ok(result);
    }
}
