public interface IEmpresaMantenimientoRepository
{
    void Crear(CrearEmpresaMantenimientoComando comando);
    void Actualizar(ActualizarEmpresaMantenimientoComando comando);
    void Eliminar(int id);
    List<EmpresaMantenimientoDto> ObtenerTodos();
}