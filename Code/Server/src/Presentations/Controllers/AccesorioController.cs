public class AccesorioController : Controller<AccesorioDto, AccesorioService, CrearAccesorioComando, ActualizarAccesorioComando, EliminarAccesorioComando>
{
    public AccesorioController(AccesorioService servicio) : base(servicio) { }
}
