using Microsoft.AspNetCore.Mvc;
using IMT_Reservas.Server.Application.Features.Usuario;
using IMT_Reservas.Server.Application.Common;
using IMT_Reservas.Server.Application.Dtos;
using IMT_Reservas.Server.Application.Features.Usuario.Dtos;
using UsuarioEntity = IMT_Reservas.Server.Core.Entities.Usuario;
using AutoMapper;

namespace IMT_Reservas.Server.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly UsuarioService _service;
    private readonly IMapper _mapper;

    public UsuarioController(UsuarioService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllUsers();
        return result.IsSuccess
            ? Ok(new Response<List<UsuarioListDto>> { Success = true, Data = result.Value })
            : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{carnet}")]
    public async Task<IActionResult> Get(string carnet)
    {
        var result = await _service.Get(carnet);
        return result.IsSuccess
            ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value })
            : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioDto dto)
    {
        var entity = _mapper.Map<UsuarioEntity>(dto);
        var result = await _service.Create(entity);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { carnet = result.Value?.Carnet }, new Response<UsuarioDetailDto> { Success = true, Data = result.Value })
            : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{carnet}")]
    public async Task<IActionResult> Update(string carnet, [FromBody] UsuarioDto dto)
    {
        var entity = _mapper.Map<UsuarioEntity>(dto);
        entity.Carnet = carnet;
        var result = await _service.Update(entity);
        return result.IsSuccess
            ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value })
            : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{carnet}")]
    public async Task<IActionResult> Delete(string carnet)
    {
        var result = await _service.Delete(carnet);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost("login")]
    public async Task<IActionResult> InitiateSession([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
            return BadRequest(new Response<object> { Success = false, Errors = new List<string> { "Email and password are required" } });

        var result = await _service.InitiateSession(request.Email, request.Password);
        return result.IsSuccess
            ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value })
            : Unauthorized(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
