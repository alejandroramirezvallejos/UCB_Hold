using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Mantenimiento;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Mantenimiento.Dtos;
using MantenimientoEntity = IMT_Reservas.Server.Core.Entities.Mantenimiento;
using AutoMapper;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase
{
    private readonly MantenimientoService _service;
    private readonly IMapper _mapper;

    public MantenimientoController(MantenimientoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

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
    public async Task<IActionResult> Create([FromBody] MantenimientoRequest dto)
    {
        var entity = _mapper.Map<MantenimientoEntity>(dto);
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<MantenimientoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MantenimientoRequest dto)
    {
        var entity = _mapper.Map<MantenimientoEntity>(dto);
        entity.Id = id;
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
