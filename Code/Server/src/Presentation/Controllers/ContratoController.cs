using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratoController : ControllerBase
{
    private readonly ContratoService _contratoService;

    public ContratoController(ContratoService contratoService)
        => _contratoService = contratoService;

    [HttpPost("crear")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ContratoCreateRequest request)
    {
        if (request?.Archivo == null || request.Archivo.Length == 0)
            return BadRequest(new Response<object>
            {
                Status = 400,
                Errors = ["Archivo requerido"]
            });

        string contenido;

        using (var reader = new StreamReader(request.Archivo.OpenReadStream()))
            contenido = await reader.ReadToEndAsync();

        var resultado = await _contratoService.Create(request.PrestamoId, contenido);

        return resultado.IsSuccess
            ? Ok(new Response<object> { Status = 200, Value = resultado.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = resultado.Errors.ToList() });
    }

    [HttpGet("{prestamoId}")]
    public async Task<IActionResult> GetByPrestamoId(int prestamoId)
    {
        var resultado = await _contratoService.GetByPrestamoId(prestamoId);

        return resultado.IsSuccess
            ? Ok(new Response<object> { Status = 200, Value = resultado.Value })
            : NotFound(new Response<object> { Status = 404, Errors = resultado.Errors.ToList() });
    }

    [HttpDelete("{prestamoId}")]
    public async Task<IActionResult> Delete(int prestamoId)
    {
        var resultado = await _contratoService.Delete(prestamoId);

        return resultado.IsSuccess
            ? NoContent()
            : BadRequest(new Response<object> { Status = 400, Errors = resultado.Errors.ToList() });
    }
}
