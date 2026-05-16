using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Application.Features.Prestamo;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class PrestamoController : Controller
{
    private readonly PrestamoService _service;
    private readonly ContratoService _contratoService;

    public PrestamoController(PrestamoService service, ContratoService contratoService)
    {
        _service = service;
        _contratoService = contratoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => ToResponse(await _service.Get(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrestamoDto request)
    {
        var result = await _service.Create(request);
        return ToCreatedResponse(result, nameof(Get), new { id = result.Value?.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PrestamoDto dto)
        => ToResponse(await _service.Update(id, dto));

    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        => ToResponse(await _service.UpdateStatus(id, request.EstadoPrestamo ?? string.Empty));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => ToDeleteResponse(await _service.Delete(id));

    [HttpGet("historial")]
    public async Task<IActionResult> GetHistory([FromQuery] string carnetUsuario, [FromQuery] string estadoPrestamo)
        => ToResponse(await _service.GetHistory(carnetUsuario, estadoPrestamo));

    [HttpGet("contrato/{prestamoId}")]
    public async Task<IActionResult> GetContrato(int prestamoId)
    {
        var result = await _contratoService.GetByPrestamoId(prestamoId);

        if (!result.IsSuccess)
            return ToResponse(result);

        return Ok(new { contrato = result.Value!.ContratoHtml });
    }
}

public record UpdateStatusRequest(string? EstadoPrestamo);
