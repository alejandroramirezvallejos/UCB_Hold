using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento.Dtos;
using EmpresaMantenimientoEntity = IMT_Reservas.Server.Core.Entities.EmpresaMantenimiento;
using AutoMapper;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly EmpresaMantenimientoService _service;
    private readonly IMapper _mapper;

    public EmpresaMantenimientoController(EmpresaMantenimientoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<EmpresaMantenimientoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmpresaMantenimientoRequest dto)
    {
        var entity = _mapper.Map<EmpresaMantenimientoEntity>(dto);
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<EmpresaMantenimientoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmpresaMantenimientoRequest dto)
    {
        var entity = _mapper.Map<EmpresaMantenimientoEntity>(dto);
        entity.Id = id;
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
