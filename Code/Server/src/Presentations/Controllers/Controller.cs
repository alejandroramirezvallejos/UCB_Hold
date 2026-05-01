using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public abstract class Controller<TDto, TService, TCrear, TActualizar, TEliminar> : ControllerBase
    where TDto : Dto
    where TService : Service<TDto>, ICrud<TDto, TCrear, TActualizar, TEliminar>
{
    protected readonly TService Servicio;

    protected Controller(TService servicio) => Servicio = servicio;

    [HttpGet]
    public Result<List<TDto?>> ObtenerTodos() => Servicio.ObtenerTodos();

    [HttpPost]
    public Result<TDto?> Crear([FromBody] TCrear comando) => Servicio.Crear(comando);

    [HttpPut]
    public Result<TDto?> Actualizar([FromBody] TActualizar comando) => Servicio.Actualizar(comando);

    [HttpDelete("{id}")]
    public Result<TDto?> Eliminar(int id)
    {
        var comando = (TEliminar)Activator.CreateInstance(typeof(TEliminar), id)!;
        return Servicio.Eliminar(comando);
    }
}
