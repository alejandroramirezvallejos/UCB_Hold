using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Prestamo;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Prestamo.Dtos;
using PrestamoEntity = IMT_Reservas.Server.Core.Entities.Prestamo;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly PrestamoService _service;

    public PrestamoController(PrestamoService service)
    {
        _service = service;
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
        var entity = new PrestamoEntity
        {
            Carnet = request.CarnetUsuario,
            FechaSolicitud = DateTime.UtcNow,
            FechaPrestamo = request.FechaPrestamo ?? request.FechaPrestamoEsperada,
            FechaPrestamoEsperada = request.FechaPrestamoEsperada ?? DateTime.UtcNow,
            FechaDevolucion = request.FechaDevolucion,
            FechaDevolucionEsperada = request.FechaDevolucionEsperada ?? DateTime.UtcNow.AddDays(7),
            Observacion = request.Observacion,
            EstadoPrestamo = "pendiente",
            IdContrato = request.IdContrato
        };
        var result = await _service.Create(entity);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<PrestamoDto> { Status = 201, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PrestamoDto dto)
    {
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
            EstadoPrestamo = dto.EstadoPrestamo,
            IdContrato = dto.IdContrato
        };
        var result = await _service.Update(entity);
        return result.IsSuccess ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
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
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SaveContrato(int id, [FromForm] ContratoPrestamoDto request)
    {
        if (request?.Contrato == null || request.Contrato.Length == 0)
            return BadRequest(new Response<object> { Status = 400, Errors = new List<string> { "Archivo de contrato requerido" } });

        byte[] contratoBytes;
        
        using (var ms = new MemoryStream())
        {
            await request.Contrato.CopyToAsync(ms);
            contratoBytes = ms.ToArray();
        }

        var result = await _service.SaveContrato(id, contratoBytes);
        
        return result.IsSuccess
            ? Ok(new Response<PrestamoDto> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
