using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Abstraction;
namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _service;

    public UsuarioController(UsuarioService service) =>
        _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAll();

        return result.IsSuccess
            ? Ok(new Response<List<UsuarioDto>> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpGet("{carnet}")]
    public async Task<IActionResult> Get(string carnet)
    {
        var result = await _service.Get(carnet);

        return result.IsSuccess
            ? Ok(new Response<UsuarioDto> { Status = 200, Value = result.Value })
            : NotFound(new Response<object> { Status = 404, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioDto dto)
    {
        var result = await _service.Create(dto);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { carnet = result.Value?.Carnet }, new Response<UsuarioDto> { Status = 201, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPut("{carnet}")]
    public async Task<IActionResult> Update(string carnet, [FromBody] UsuarioDto dto)
    {
        var result = await _service.Update(carnet, dto);

        return result.IsSuccess
            ? Ok(new Response<UsuarioDto> { Status = 200, Value = result.Value })
            : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{carnet}")]
    public async Task<IActionResult> Delete(string carnet)
    {
        var result = await _service.Delete(carnet);
       
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Status = 400, Errors = result.Errors.ToList() });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Contrasena))
            return BadRequest(new Response<object> { Status = 400, Errors = new List<string> { "Email and password are required" } });

        var result = await _service.Login(request.Email, request.Contrasena);

        return result.IsSuccess
            ? Ok(new Response<UsuarioDto> { Status = 200, Value = result.Value })
            : Unauthorized(new Response<object> { Status = 401, Errors = result.Errors.ToList() });
    }
}
