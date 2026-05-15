using IMT_Reservas.Server.Application.Features.Componente;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class ComponenteController : Controller
{
    private readonly ComponenteService _service;

    public ComponenteController(ComponenteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _service.GetAll());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => ToResponse(await _service.Get(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ComponenteDto dto)
    {
        var result = await _service.Create(dto);
        return ToCreatedResponse(result, nameof(Get), new { id = result.Value?.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ComponenteDto dto)
        => ToResponse(await _service.Update(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => ToDeleteResponse(await _service.Delete(id));
}
