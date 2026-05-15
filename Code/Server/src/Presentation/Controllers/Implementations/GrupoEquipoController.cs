using IMT_Reservas.Server.Application.Features.GrupoEquipo;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Route("api/[controller]")]
public class GrupoEquipoController : Controller
{
    private readonly GrupoEquipoService _service;

    public GrupoEquipoController(GrupoEquipoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _service.GetAll());

    [HttpGet("buscar")]
    public async Task<IActionResult> Search([FromQuery] string? nombre = null, [FromQuery] string? categoria = null)
        => ToResponse(await _service.Search(nombre, categoria));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
        => ToResponse(await _service.Get(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GrupoEquipoDto dto)
    {
        var result = await _service.Create(dto);
       
        return ToCreatedResponse(result, nameof(Get), new { id = result.Value?.Id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] GrupoEquipoDto dto)
        => ToResponse(await _service.Update(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => ToDeleteResponse(await _service.Delete(id));
}
