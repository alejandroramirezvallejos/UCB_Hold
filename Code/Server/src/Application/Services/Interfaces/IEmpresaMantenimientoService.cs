public interface IEmpresaMantenimientoService
{
    void CrearEmpresaMantenimiento(CrearEmpresaMantenimientoComando comando);
    List<EmpresaMantenimientoDto>? ObtenerTodasEmpresasMantenimiento();
    void ActualizarEmpresaMantenimiento(ActualizarEmpresaMantenimientoComando comando);
    void EliminarEmpresaMantenimiento(EliminarEmpresaMantenimientoComando comando);
}
