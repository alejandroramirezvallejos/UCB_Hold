using IMT_Reservas.Server.Application.Features.Usuario;
using Controller = IMT_Reservas.Server.Presentation.Controllers.Abstraction.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace IMT_Reservas.Server.Presentation.Controllers.Implementations;

[Authorize]
[Route("api/[controller]")]
public class UsuarioController : Controller
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service) => _service = service;

    [Authorize(Roles = "administrador")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _service.GetAll());

    [HttpGet("{carnet}")]
    public async Task<IActionResult> Get(string carnet)
        => ToResponse(await _service.Get(carnet));

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioDto dto)
    {
        var result = await _service.Create(dto);
        return ToCreatedResponse(result, nameof(Get), new { carnet = result.Value?.Carnet });
    }

    [HttpPut("{carnet}")]
    public async Task<IActionResult> Update(string carnet, [FromBody] UsuarioDto dto)
        => ToResponse(await _service.Update(carnet, dto));

    [Authorize(Roles = "administrador")]
    [HttpDelete("{carnet}")]
    public async Task<IActionResult> Delete(string carnet)
        => ToDeleteResponse(await _service.Delete(carnet));

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioDto request)
        => ToResponse(await _service.Login(request.Email ?? string.Empty, request.Contrasena ?? string.Empty));

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto request)
        => ToResponse(await _service.Refresh(request.RefreshToken));
}
