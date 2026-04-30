using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class GaveteroController : ControllerBase
{
    private readonly IGaveteroService _servicio;
    public GaveteroController(IGaveteroService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<GaveteroDto> Crear([FromBody] CrearGaveteroComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<GaveteroDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<GaveteroDto> Actualizar([FromBody] ActualizarGaveteroComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<GaveteroDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarGaveteroComando(id));
    }
}
