using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class AccesorioController : Controller<AccesorioDto, AccesorioService, CrearAccesorioComando, ActualizarAccesorioComando, EliminarAccesorioComando>
{
    public AccesorioController(AccesorioService servicio) : base(servicio) { }
}
