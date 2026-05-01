public class MuebleController : Controller<MuebleDto, MuebleService, CrearMuebleComando, ActualizarMuebleComando, EliminarMuebleComando>
{
    public MuebleController(MuebleService servicio) : base(servicio) { }
}
