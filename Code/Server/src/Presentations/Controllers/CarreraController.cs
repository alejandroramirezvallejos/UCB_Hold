public class CarreraController : Controller<CarreraDto, CarreraService, CrearCarreraComando, ActualizarCarreraComando, EliminarCarreraComando>
{
    public CarreraController(CarreraService servicio) : base(servicio) { }
}
