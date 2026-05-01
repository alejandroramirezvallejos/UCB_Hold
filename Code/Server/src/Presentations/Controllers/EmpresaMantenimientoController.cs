public class EmpresaMantenimientoController : Controller<EmpresaMantenimientoDto, EmpresaMantenimientoService, CrearEmpresaMantenimientoComando, ActualizarEmpresaMantenimientoComando, EliminarEmpresaMantenimientoComando>
{
    public EmpresaMantenimientoController(EmpresaMantenimientoService servicio) : base(servicio) { }
}
