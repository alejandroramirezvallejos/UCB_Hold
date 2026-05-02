using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Carrera;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Dtos;
using IMT_Reservas.Server.Application.Features.Carrera.Dtos;
using CarreraEntity = IMT_Reservas.Server.Core.Entities.Carrera;
using AutoMapper;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarreraController : ControllerBase
{
    private readonly CarreraService _service;
    private readonly IMapper _mapper;

    public CarreraController(CarreraService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _service.GetAll();
        return result.IsSuccess ? Ok(new Response<List<CarreraListDto>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var result = _service.Get(id);
        return result.IsSuccess ? Ok(new Response<CarreraDetailDto> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CarreraDto dto)
    {
        var entity = _mapper.Map<CarreraEntity>(dto);
        var result = _service.Create(entity);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<CarreraDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CarreraDto dto)
    {
        var entity = _mapper.Map<CarreraEntity>(dto);
        entity.Id = id;
        var result = _service.Update(entity);
        return result.IsSuccess ? Ok(new Response<CarreraDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _service.Delete(id);
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
