using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class EquipoController : Controller<EquipoDto, EquipoService, CrearEquipoComando, ActualizarEquipoComando, EliminarEquipoComando>
{
    public EquipoController(EquipoService servicio) : base(servicio) { }
}
