using IMT_Reservas.Server.Application.Features.Contrato;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class ContratoController : Controller
{
    private readonly ContratoService _contratoService;

    public ContratoController(ContratoService contratoService) => _contratoService = contratoService;

    [HttpPost("crear")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] int? prestamoId, IFormFile? archivo)
    {
        string? contenido = null;

        if (archivo != null)
        {
            using var reader = new StreamReader(archivo.OpenReadStream());
            contenido = await reader.ReadToEndAsync();
        }

        var resultado = await _contratoService.CreateForPrestamo(prestamoId ?? 0, contenido ?? "");
        
        return ToResponse(resultado);
    }

    [HttpGet("{prestamoId}")]
    public async Task<IActionResult> GetByPrestamoId(int prestamoId)
        => ToResponse(await _contratoService.GetByPrestamoId(prestamoId));

    [HttpDelete("{prestamoId}")]
    public async Task<IActionResult> Delete(int prestamoId)
        => ToDeleteResponse(await _contratoService.Delete(prestamoId));
}
