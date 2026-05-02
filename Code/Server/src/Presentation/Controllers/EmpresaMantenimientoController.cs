using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.EmpresaMantenimiento;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Dtos;
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
    public IActionResult GetAll()
    {
        var result = _service.GetAll();
        return result.IsSuccess ? Ok(new Response<List<EmpresaMantenimientoListDto>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var result = _service.Get(id);
        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDetailDto> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public IActionResult Create([FromBody] EmpresaMantenimientoDto dto)
    {
        var entity = _mapper.Map<EmpresaMantenimientoEntity>(dto);
        var result = _service.Create(entity);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<EmpresaMantenimientoDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] EmpresaMantenimientoDto dto)
    {
        var entity = _mapper.Map<EmpresaMantenimientoEntity>(dto);
        entity.Id = id;
        var result = _service.Update(entity);
        return result.IsSuccess ? Ok(new Response<EmpresaMantenimientoDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _service.Delete(id);
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
