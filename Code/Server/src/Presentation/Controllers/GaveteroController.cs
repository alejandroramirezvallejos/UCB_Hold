using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Gavetero;
using IMT_Reservas.Server.Application.Abstraction;
using GaveteroEntity = IMT_Reservas.Server.Core.Entities.Gavetero;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaveteroController : ControllerBase
{
    private readonly GaveteroService _service;

    public GaveteroController(GaveteroService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<GaveteroDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<GaveteroDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GaveteroDto dto)
    {
        var muebleId = await _service.ResolveMuebleId(dto.NombreMueble);

        var entity = new GaveteroEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Tipo = dto.Tipo,
            IdMueble = muebleId ?? 0,
            Longitud = dto.Longitud,
            Profundidad = dto.Profundidad,
            Altura = dto.Altura
        };
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<GaveteroDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] GaveteroDto dto)
    {
        var muebleId = await _service.ResolveMuebleId(dto.NombreMueble, id);

        var entity = new GaveteroEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Tipo = dto.Tipo,
            IdMueble = muebleId ?? 0,
            Longitud = dto.Longitud,
            Profundidad = dto.Profundidad,
            Altura = dto.Altura
        };
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<GaveteroDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
