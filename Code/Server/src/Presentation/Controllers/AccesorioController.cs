using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Accesorio;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Accesorio.Dtos;
using AccesorioEntity = IMT_Reservas.Server.Core.Entities.Accesorio;
using AutoMapper;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccesorioController : ControllerBase
{
    private readonly AccesorioService _service;
    private readonly IMapper _mapper;

    public AccesorioController(AccesorioService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        
        return result.IsSuccess ? Ok(new Response<List<AccesorioListDto>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        
        return result.IsSuccess ? Ok(new Response<AccesorioDetailDto> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccesorioDto dto)
    {
        var entity = _mapper.Map<AccesorioEntity>(dto);
        var result = await _service.Create(entity);
        
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<AccesorioDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AccesorioDto dto)
    {
        var entity = _mapper.Map<AccesorioEntity>(dto);
        entity.Id = id;
        var result = await _service.Update(entity);
        
        return result.IsSuccess ? Ok(new Response<AccesorioDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
