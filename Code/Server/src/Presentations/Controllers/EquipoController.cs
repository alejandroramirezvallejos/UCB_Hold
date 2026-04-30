using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class EquipoController : ControllerBase
{
    private readonly IEquipoService _servicio;
    public EquipoController(IEquipoService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<EquipoDto> Crear([FromBody] CrearEquipoComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<EquipoDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<EquipoDto> Actualizar([FromBody] ActualizarEquipoComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<EquipoDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarEquipoComando(id));
    }
}
