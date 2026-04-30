using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class CarreraController : ControllerBase
{
    private readonly ICarreraService _servicio;
    public CarreraController(ICarreraService servicio) => _servicio = servicio;

    [HttpGet]
    public Result<List<CarreraDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPost]
    public Result<CarreraDto> Crear([FromBody] CrearCarreraComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpPut]
    public Result<CarreraDto> Actualizar([FromBody] ActualizarCarreraComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<CarreraDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarCarreraComando(id));
    }
}
