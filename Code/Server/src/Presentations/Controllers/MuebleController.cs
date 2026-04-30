using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class MuebleController : ControllerBase
{
    private readonly IMuebleService _servicio;
    public MuebleController(IMuebleService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<MuebleDto> Crear([FromBody] CrearMuebleComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<MuebleDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<MuebleDto> Actualizar([FromBody] ActualizarMuebleComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<MuebleDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarMuebleComando(id));
    }
}
