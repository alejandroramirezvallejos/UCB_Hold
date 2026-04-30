using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class EmpresaMantenimientoController : ControllerBase
{
    private readonly IEmpresaMantenimientoService _servicio;
    public EmpresaMantenimientoController(IEmpresaMantenimientoService servicio) => _servicio = servicio;

    [HttpPost]
    public Result<EmpresaMantenimientoDto> Crear([FromBody] CrearEmpresaMantenimientoComando input)
    {
        return _servicio.Crear(input);
    }

    [HttpGet]
    public Result<List<EmpresaMantenimientoDto>> ObtenerTodos()
    {
        return _servicio.ObtenerTodos();
    }

    [HttpPut]
    public Result<EmpresaMantenimientoDto> Actualizar([FromBody] ActualizarEmpresaMantenimientoComando input)
    {
        return _servicio.Actualizar(input);
    }

    [HttpDelete("{id}")]
    public Result<EmpresaMantenimientoDto> Eliminar(int id)
    {
        return _servicio.Eliminar(new EliminarEmpresaMantenimientoComando(id));
    }
}
