public class ComponenteController : Controller<ComponenteDto, ComponenteService, CrearComponenteComando, ActualizarComponenteComando, EliminarComponenteComando>
{
    public ComponenteController(ComponenteService servicio) : base(servicio) { }
}
