using Microsoft.AspNetCore.Mvc;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using IMT_Reservas.Server.Shared.Common;
using IMT_Reservas.Server.Application.RequestDTOs.Carrito;
using IMT_Reservas.Server.Application.ResponseDTOs;
using IMT_Reservas.Server.Application.Services.Implementations;

[ApiController]
[Route("api/[controller]")]
[TranslateResultToActionResult]
public class CarritoController : ControllerBase
{
    private readonly CarritoService _servicio;
    public CarritoController(CarritoService servicio) => _servicio = servicio;

    [HttpPost("fechasNoDisponibles")]
    public Result<List<FechaNoDisponibleDto>> ObtenerFechasNoDisponibles([FromBody] ObtenerFechasNoDisponiblesComando input) =>
        _servicio.ObtenerFechasNoDisponibles(input);

    [HttpPost("disponibilidadEquipos")]
    public Result<List<DisponibilidadEquipoDto>> ObtenerDisponibilidadEquiposPorFechasYGrupos([FromBody] ObtenerDisponibilidadEquiposComando input) =>
        _servicio.ObtenerDisponibilidadEquiposPorFechasYGrupos(input);
}
