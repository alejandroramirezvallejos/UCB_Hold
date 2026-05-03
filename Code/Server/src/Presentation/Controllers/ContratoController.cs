using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Contrato;
using IMT_Reservas.Server.Application.Features.Contrato.Dtos;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Archivo;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratoController : ControllerBase
{
    private readonly ContratoService _contratoService;
    private readonly ArchivoService _archivoService;

    public ContratoController(ContratoService contratoService, ArchivoService archivoService)
    {
        _contratoService = contratoService;
        _archivoService = archivoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(int prestamoId, IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new Response<object> { Success = false, Errors = new List<string> { "Archivo requerido" } });

        var resultado = await _contratoService.Create(prestamoId, archivo.OpenReadStream(), archivo.FileName);
        
        return resultado.IsSuccess
            ? Ok(new Response<ContratoDetail> { Success = true, Data = resultado.Value })
            : BadRequest(new Response<object> { Success = false, Errors = resultado.Errors.ToList() });
    }

    [HttpGet("{prestamoId}")]
    public async Task<IActionResult> Get(int prestamoId)
    {
        var resultado = await _contratoService.Get(prestamoId);
        
        return resultado.IsSuccess
            ? Ok(new Response<ContratoDetail> { Success = true, Data = resultado.Value })
            : NotFound(new Response<object> { Success = false, Errors = resultado.Errors.ToList() });
    }

    [HttpDelete("{prestamoId}")]
    public async Task<IActionResult> Delete(int prestamoId)
    {
        var resultado = await _contratoService.Delete(prestamoId);
        
        return resultado.IsSuccess
            ? Ok(new Response<object> { Success = true })
            : BadRequest(new Response<object> { Success = false, Errors = resultado.Errors.ToList() });
    }

    [HttpGet("{prestamoId}/download")]
    public async Task<IActionResult> Download(int prestamoId)
    {
        var obtenerResultado = await _contratoService.Get(prestamoId);
        
        if (!obtenerResultado.IsSuccess)
            return NotFound();

        if (string.IsNullOrEmpty(obtenerResultado.Value.FileId))
            return BadRequest();

        var descargarResultado = await _archivoService.Download(obtenerResultado.Value.FileId);
        
        if (!descargarResultado.IsSuccess)
            return BadRequest();

        return File(descargarResultado.Value, "text/html", "contrato.html");
    }
}
