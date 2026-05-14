using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Abstraction;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly MantenimientoService _service;

    public MantenimientoController(MantenimientoService service) => 
        _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<MantenimientoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<MantenimientoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MantenimientoDto dto)
    {
        var result = await _service.CreateWithDetalles(dto);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<MantenimientoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MantenimientoDto dto)
    {
        var entity = new MantenimientoEntity
        {
            Id                      = id,
            IdEmpresa               = dto.IdEmpresa ?? 0,
            FechaMantenimiento      = dto.FechaMantenimiento ?? DateTime.UtcNow,
            FechaFinalMantenimiento = dto.FechaFinalMantenimiento ?? DateTime.UtcNow,
            Descripcion             = dto.Descripcion,
            Costo                   = dto.Costo
        };
        
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<MantenimientoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
