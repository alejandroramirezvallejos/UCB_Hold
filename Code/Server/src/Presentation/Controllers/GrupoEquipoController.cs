using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using IMT_Reservas.Server.Application.Abstraction;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrupoEquipoController : ControllerBase
{
    private readonly GrupoEquipoService _service;

    public GrupoEquipoController(GrupoEquipoService service) => 
        _service = service ?? throw new InvalidOperationException("GrupoEquipoService not registered");

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        
        return result.IsSuccess ? Ok(new Response<List<GrupoEquipoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Search([FromQuery] string? nombre = null, [FromQuery] string? categoria = null)
    {
        var result = await _service.Search(nombre, categoria);
       
        return result.IsSuccess ? Ok(new Response<List<GrupoEquipoDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        
        return result.IsSuccess ? Ok(new Response<GrupoEquipoDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GrupoEquipoDto dto)
    {
        var result = await _service.Create(dto);
        
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<GrupoEquipoDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] GrupoEquipoDto dto)
    {
        var result = await _service.Update(id, dto);
        
        return result.IsSuccess ? Ok(new Response<GrupoEquipoDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
