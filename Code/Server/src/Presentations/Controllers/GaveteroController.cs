public class GaveteroController : Controller<GaveteroDto, GaveteroService, CrearGaveteroComando, ActualizarGaveteroComando, EliminarGaveteroComando>
{
    public GaveteroController(GaveteroService servicio) : base(servicio) { }
}
