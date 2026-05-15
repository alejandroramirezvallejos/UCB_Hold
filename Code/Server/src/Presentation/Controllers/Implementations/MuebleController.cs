using IMT_Reservas.Server.Application.Abstraction;
using IMT_Reservas.Server.Application.Features.Mueble;
using IMT_Reservas.Server.Infrastructure.Repositories.Implementations;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
using MuebleEntity = IMT_Reservas.Server.Core.Entities.Mueble;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class MuebleController : Controller
{
    private readonly Service<MuebleEntity, MuebleRepository, MuebleDto> _service;

    public MuebleController(Service<MuebleEntity, MuebleRepository, MuebleDto> service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => ToResponse(await _service.Get(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MuebleDto dto)
    {
        var result = await _service.Create(dto);
        
        return ToCreatedResponse(result, nameof(Get), new { id = result.Value?.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MuebleDto dto)
        => ToResponse(await _service.Update(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => ToDeleteResponse(await _service.Delete(id));
}
