using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Categoria;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Features.Categoria.Dtos;
using CategoriaEntity = IMT_Reservas.Server.Core.Entities.Categoria;
using AutoMapper;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly CategoriaService _service;
    private readonly IMapper _mapper;

    public CategoriaController(CategoriaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        return result.IsSuccess ? Ok(new Response<List<CategoriaListDto>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        return result.IsSuccess ? Ok(new Response<CategoriaDetailDto> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoriaDto dto)
    {
        var entity = _mapper.Map<CategoriaEntity>(dto);
        var result = await _service.Create(entity);
        
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<CategoriaDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoriaDto dto)
    {
        var entity = _mapper.Map<CategoriaEntity>(dto);
        entity.Id = id;
        var result = await _service.Update(entity);
        
        return result.IsSuccess ? Ok(new Response<CategoriaDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
       
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}
