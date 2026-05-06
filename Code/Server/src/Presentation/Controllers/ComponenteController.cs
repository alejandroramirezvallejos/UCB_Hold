using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Componente;
using IMT_Reservas.Server.Application.Abstraction;
using ComponenteEntity = IMT_Reservas.Server.Core.Entities.Componente;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComponenteController : ControllerBase
{
    private readonly ComponenteService _service;

    public ComponenteController(ComponenteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess ? Ok(new Response<List<ComponenteDto>> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.Get(id);

        return result.IsSuccess ? Ok(new Response<ComponenteDto> { Status = 200, Value = result.Value }) : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ComponenteDto dto)
    {
        var equipoId = await _service.ResolveEquipoId(dto.IdEquipo, dto.CodigoImtEquipo);

        var entity = new ComponenteEntity
        {
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            PrecioReferencia = dto.PrecioReferencia,
            IdEquipo = equipoId ?? 0,
            UrlDataSheet = dto.UrlDataSheet
        };
        var result = await _service.Create(entity);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<ComponenteDto> { Status = 201, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ComponenteDto dto)
    {
        var equipoId = await _service.ResolveEquipoId(dto.IdEquipo, dto.CodigoImtEquipo);

        var entity = new ComponenteEntity
        {
            Id = id,
            Nombre = dto.Nombre ?? string.Empty,
            Modelo = dto.Modelo ?? string.Empty,
            Tipo = dto.Tipo,
            Descripcion = dto.Descripcion,
            PrecioReferencia = dto.PrecioReferencia,
            IdEquipo = equipoId ?? 0,
            UrlDataSheet = dto.UrlDataSheet
        };
        var result = await _service.Update(entity);

        return result.IsSuccess ? Ok(new Response<ComponenteDto> { Status = 200, Value = result.Value }) : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);

        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }
}
