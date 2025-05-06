using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly ICrearPrestamoComando      _crear;
    private readonly IObtenerPrestamoConsulta   _obtener;
    private readonly IActualizarPrestamoComando _actualizar;
    private readonly IEliminarPrestamoComando   _eliminar;

    public PrestamoController(ICrearPrestamoComando crear, IObtenerPrestamoConsulta obtener,
                              IActualizarPrestamoComando actualizar, IEliminarPrestamoComando eliminar)
    {
        _crear      = crear;
        _obtener    = obtener;
        _actualizar = actualizar;
        _eliminar   = eliminar;
    }

    [HttpGet("{id}")]
    public ActionResult<PrestamoDto> ObtenerPorId(int id)
    {
        PrestamoDto? resultado = _obtener.Handle(new ObtenerPrestamoConsulta(id));
        if (resultado == null)
            return NotFound($"No se encontro el prestamo con ID {id}");

        return Ok(resultado);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] PrestamoRequestDto solicitud)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        CrearPrestamoComando comando = new CrearPrestamoComando(
            solicitud.FechaSolicitud,
            solicitud.FechaPrestamo,
            solicitud.FechaDevolucion,
            solicitud.FechaDevolucionEsperada,
            solicitud.Observacion,
            solicitud.EstadoPrestamo,
            solicitud.CarnetUsuario,
            solicitud.EquipoId
        );

        PrestamoDto resultado = _crear.Handle(comando);
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
    public IActionResult Actualizar(int id, [FromBody] PrestamoRequestDto solicitud)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        ActualizarPrestamoComando comando = new ActualizarPrestamoComando(
            id,
            solicitud.FechaSolicitud,
            solicitud.FechaPrestamo,
            solicitud.FechaDevolucion,
            solicitud.FechaDevolucionEsperada,
            solicitud.Observacion,
            solicitud.EstadoPrestamo,
            solicitud.CarnetUsuario,
            solicitud.EquipoId
        );

        PrestamoDto? resultado = _actualizar.Handle(comando);
        if (resultado == null)
            return NotFound($"No se encontro el prestamo con ID {id} para actualizar");

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    public IActionResult Eliminar(int id)
    {
        bool exito = _eliminar.Handle(new EliminarPrestamoComando(id));
        if (!exito)
            return NotFound($"No se encontro el prestamo con ID {id} para eliminar");

        return NoContent();
    }
}
