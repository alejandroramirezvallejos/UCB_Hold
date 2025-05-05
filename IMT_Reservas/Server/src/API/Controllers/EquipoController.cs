using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly ICrearEquipoComando      _crear;
    private readonly IObtenerEquipoConsulta   _obtener;
    private readonly IActualizarEquipoComando _actualizar;
    private readonly IEliminarEquipoComando   _eliminar;

    public EquiposController(ICrearEquipoComando crear, IObtenerEquipoConsulta obtener,
                             IActualizarEquipoComando actualizar, IEliminarEquipoComando eliminar)
    {
        _crear       = crear;
        _obtener     = obtener;
        _actualizar  = actualizar;
        _eliminar    = eliminar;
    }

    [HttpGet("{id}")]
    public ActionResult<EquipoDto> ObtenerPorId(int id)
    {
        EquipoDto? resultado = _obtener.Handle(new ObtenerEquipoConsulta(id));
        if (resultado == null)
            return NotFound($"No se encontro el equipo con ID {id}");

        return Ok(resultado);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] EquipoRequestDto solicitud)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CrearEquipoComando comando = new CrearEquipoComando(
            solicitud.GrupoEquipoId,
            solicitud.CodigoImt,
            solicitud.CodigoUcb,
            solicitud.Descripcion,
            solicitud.EstadoEquipo,
            solicitud.NumeroSerial,
            solicitud.Ubicacion,
            solicitud.CostoReferencia,
            solicitud.TiempoMaximoPrestamo,
            solicitud.Procedencia,
            solicitud.GaveteroId
        );

        EquipoDto resultado = _crear.Handle(comando);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new
            { 
                id = resultado.Id 
            },
            resultado
        );
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] EquipoRequestDto solicitud)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ActualizarEquipoComando comando = new ActualizarEquipoComando(
            id,
            solicitud.GrupoEquipoId,
            solicitud.CodigoImt,
            solicitud.CodigoUcb,
            solicitud.Descripcion,
            solicitud.EstadoEquipo,
            solicitud.NumeroSerial,
            solicitud.Ubicacion,
            solicitud.CostoReferencia,
            solicitud.TiempoMaximoPrestamo,
            solicitud.Procedencia,
            solicitud.GaveteroId
        );

        EquipoDto? resultado = _actualizar.Handle(comando);

        if (resultado == null)
        {
            return NotFound($"No se encontro el equipo con ID {id} para actualizar");
        }

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        EliminarEquipoComando comando = new EliminarEquipoComando(id);
        bool exito = _eliminar.Handle(comando);

        if (!exito)
        {
            return NotFound($"No se encontro el equipo con ID {id} para eliminar");
        }

        return NoContent();
    }
}