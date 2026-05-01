using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class MuebleController : Controller<MuebleDto, MuebleService, CrearMuebleComando, ActualizarMuebleComando, EliminarMuebleComando>
{
    public MuebleController(MuebleService servicio) : base(servicio) { }
}
