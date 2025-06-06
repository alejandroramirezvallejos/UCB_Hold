//implementar
public interface IEmpresaMantenimientoRepository
{
    EmpresaMantenimientoDto Crear(CrearEmpresaMantenimientoComando comando);
    EmpresaMantenimientoDto? ObtenerPorId(int id);
    EmpresaMantenimientoDto? Actualizar(ActualizarEmpresaMantenimientoComando comando);
    bool Eliminar(int id);
    List<EmpresaMantenimientoDto> ObtenerTodos();
}