using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Core.Entities;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly PrestamoService _service;
    private readonly IMT_Reservas.Server.Application.Features.Contrato.ContratoService _contratoService;

    public PrestamoController(PrestamoService service, IMT_Reservas.Server.Application.Features.Contrato.ContratoService contratoService)
    {
        _service = service;
        _contratoService = contratoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        
        return result.IsSuccess ? Ok(new Response<List<PrestamoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        
        return result.IsSuccess ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrestamoDto request)
    {
        var result = await _service.CreateFromDto(request);
        
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<PrestamoDto> { Status = 201, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PrestamoDto dto)
    {
        var estadoPrestamo = EstadoPrestamo.Pendiente;
        
        if (!string.IsNullOrWhiteSpace(dto.EstadoPrestamo))
        {
            var lower = dto.EstadoPrestamo.ToLowerInvariant();
            
            estadoPrestamo = lower switch
            {
                "aprobado" => EstadoPrestamo.Aprobado,
                "activo" => EstadoPrestamo.Activo,
                "rechazado" => EstadoPrestamo.Rechazado,
                "finalizado" => EstadoPrestamo.Finalizado,
                "cancelado" => EstadoPrestamo.Cancelado,
                _ => EstadoPrestamo.Pendiente
            };
        }

        var entity = new PrestamoEntity
        {
            Id = id,
            Carnet = dto.CarnetUsuario,
            FechaSolicitud = dto.FechaSolicitud ?? DateTime.UtcNow,
            FechaPrestamo = dto.FechaPrestamo ?? dto.FechaPrestamoEsperada,
            FechaPrestamoEsperada = dto.FechaPrestamoEsperada ?? DateTime.UtcNow,
            FechaDevolucion = dto.FechaDevolucion,
            FechaDevolucionEsperada = dto.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7),
            Observacion = dto.Observacion,
            EstadoPrestamo = estadoPrestamo,
            IdContrato = dto.IdContrato
        };
        
        var result = await _service.Update(entity);
        
        return result.IsSuccess ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> UpdateEstado(int id, [FromBody] EstadoRequest request)
    {
        var result = await _service.UpdateEstado(id, request.EstadoPrestamo ?? string.Empty);

        return result.IsSuccess
            ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("historial")]
    public async Task<IActionResult> GetHistorial([FromQuery] string carnetUsuario, [FromQuery] string estadoPrestamo)
    {
        var result = await _service.GetHistorial(carnetUsuario, estadoPrestamo);

        return result.IsSuccess
            ? Ok(new Response<List<PrestamoDto>> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPost("{id:int}/contrato")]
    public async Task<IActionResult> SaveContrato(int id, [FromBody] SaveContratoForm form)
    {
        byte[] contratoBytes = [];

        if (!string.IsNullOrEmpty(form.ContratoHtml))
        {
            contratoBytes = System.Text.Encoding.UTF8.GetBytes(form.ContratoHtml);
        }

        var result = await _service.SaveContrato(id, contratoBytes);

        return result.IsSuccess
            ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("contrato/{prestamoId}")]
    public async Task<IActionResult> ObtenerContratoPorPrestamo(int prestamoId)
    {
        var result = await _contratoService.GetByPrestamoId(prestamoId);

        if (!result.IsSuccess)
            return NotFound(new { mensaje = $"No se encontró contrato para el préstamo {prestamoId}" });

        return Ok(new { contrato = result.Value.ContratoHtml });
    }
}

public sealed record EstadoRequest(string? EstadoPrestamo);

// Wrapper para el string HTML
public sealed class SaveContratoForm
{
    public string? ContratoHtml { get; set; }
}
