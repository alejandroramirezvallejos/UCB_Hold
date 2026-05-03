using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Gavetero.Dtos;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
using AutoMapper;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly GaveteroService _service;
    private readonly IMapper _mapper;

    public GaveteroController(GaveteroService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        
        return result.IsSuccess ? Ok(new Response<List<GaveteroList>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        
        return result.IsSuccess ? Ok(new Response<GaveteroDetail> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GaveteroRequest dto)
    {
        var entity = _mapper.Map<GaveteroEntity>(dto);
        var result = await _service.Create(entity);
        
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<GaveteroDetail> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] GaveteroRequest dto)
    {
        var entity = _mapper.Map<GaveteroEntity>(dto);
        entity.Id = id;
        var result = await _service.Update(entity);
        
        return result.IsSuccess ? Ok(new Response<GaveteroDetail> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
