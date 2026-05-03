using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using IMT_Reservas.Server.Application.Abstraction;
using ContratoEntity = IMT_Reservas.Server.Core.Entities.Contrato;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratoController : ControllerBase
{
    private readonly ContratoService _service;

    public ContratoController(ContratoService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create(int prestamoId)
    {
        var entity = new ContratoEntity { PrestamoId = prestamoId, FechaCreacion = DateTime.UtcNow };
        var resultado = await _service.Create(entity);

        return resultado.IsSuccess
            ? Ok(new Response<ContratoDto> { Status = 200, Value = resultado.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = resultado.Errors.ToList() });
    }

    [HttpGet("{prestamoId}")]
    public async Task<IActionResult> GetByPrestamoId(int prestamoId)
    {
        var resultado = await _service.GetByPrestamoId(prestamoId);

        return resultado.IsSuccess
            ? Ok(new Response<ContratoDto> { Status = 200, Value = resultado.Value })
            : NotFound(new Response<object> { Status = 404, Errors = resultado.Errors.ToList() });
    }

    [HttpDelete("{prestamoId}")]
    public async Task<IActionResult> Delete(int prestamoId)
    {
        var resultado = await _service.Delete(prestamoId);

        return resultado.IsSuccess
            ? Ok(new Response<object> { Status = 200, Value = null })
            : BadRequest(new Response<object> { Status = 400, Errors = resultado.Errors.ToList() });
    }
}
