using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class CarreraController : Controller<CarreraDto, CarreraService, CrearCarreraComando, ActualizarCarreraComando, EliminarCarreraComando>
{
    public CarreraController(CarreraService servicio) : base(servicio) { }
}
