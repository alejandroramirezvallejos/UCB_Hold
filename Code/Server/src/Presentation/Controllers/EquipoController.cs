using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Equipo;
using IMT_Reservas.Server.Application.Abstraction;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipoController : ControllerBase
{
    private readonly EquipoService _service;

    public EquipoController(EquipoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();
        return result.IsSuccess
            ? Ok(new Response<List<EquipoDto>> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);
        return result.IsSuccess
            ? Ok(new Response<EquipoDto> { Status = 200, Value = result.Value })
            : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EquipoDto dto)
    {
        var result = await _service.Create(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<EquipoDto> { Status = 201, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipoDto dto)
    {
        var result = await _service.Update(id, dto);
        return result.IsSuccess
            ? Ok(new Response<EquipoDto> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
