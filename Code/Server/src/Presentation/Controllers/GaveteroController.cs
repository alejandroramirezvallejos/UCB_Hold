using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class GaveteroController : Controller<GaveteroDto, GaveteroService, CrearGaveteroComando, ActualizarGaveteroComando, EliminarGaveteroComando>
{
    public GaveteroController(GaveteroService servicio) : base(servicio) { }
}
