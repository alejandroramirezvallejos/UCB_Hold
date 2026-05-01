using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class ComponenteController : Controller<ComponenteDto, ComponenteService, CrearComponenteComando, ActualizarComponenteComando, EliminarComponenteComando>
{
    public ComponenteController(ComponenteService servicio) : base(servicio) { }
}
