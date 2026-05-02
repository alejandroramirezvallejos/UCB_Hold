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
    public IActionResult GetAll()
    {
        var result = _service.GetAllUsuarios();
        return result.IsSuccess ? Ok(new Response<List<UsuarioListDto>> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var result = _service.Get(id);
        return result.IsSuccess ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value }) : NotFound(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost]
    public IActionResult Create([FromBody] UsuarioDto dto)
    {
        var entity = _mapper.Map<UsuarioEntity>(dto);
        var result = _service.Create(entity);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value?.Id }, new Response<UsuarioDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UsuarioDto dto)
    {
        var entity = _mapper.Map<UsuarioEntity>(dto);
        entity.Id = id;
        var result = _service.Update(entity);
        return result.IsSuccess ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value }) : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _service.Delete(id);
        return result.IsSuccess ? NoContent() : BadRequest(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }

    [HttpPost("iniciarSesion")]
    public IActionResult InitiateSession([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Contrasena))
            return BadRequest(new Response<object> { Success = false, Errors = new List<string> { "Email y contraseña son requeridos" } });

        var result = _service.InitiateSession(request.Email, request.Contrasena);
        return result.IsSuccess ? Ok(new Response<UsuarioDetailDto> { Success = true, Data = result.Value }) : Unauthorized(new Response<object> { Success = false, Errors = result.Errors.ToList() });
    }
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Contrasena { get; set; }
}
