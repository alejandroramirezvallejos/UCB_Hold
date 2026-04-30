using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class AccesorioController : ControllerBase
{
    private readonly IAccesorioService _servicio;
    public AccesorioController(IAccesorioService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<AccesorioDto> Crear([FromBody] CrearAccesorioComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<AccesorioDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<AccesorioDto> Actualizar([FromBody] ActualizarAccesorioComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<AccesorioDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarAccesorioComando(id));
    }
}
