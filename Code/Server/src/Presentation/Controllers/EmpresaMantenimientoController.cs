using IMT_Reservas.Server.Application.Services.Implementations;
using IMT_Reservas.Server.Application.Commands;
using IMT_Reservas.Server.Application.DTOs.Response;
public class EmpresaMantenimientoController : Controller<EmpresaMantenimientoDto, EmpresaMantenimientoService, CrearEmpresaMantenimientoComando, ActualizarEmpresaMantenimientoComando, EliminarEmpresaMantenimientoComando>
{
    public EmpresaMantenimientoController(EmpresaMantenimientoService servicio) : base(servicio) { }
}
