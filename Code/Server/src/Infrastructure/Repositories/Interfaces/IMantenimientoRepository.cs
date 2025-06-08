public interface IMantenimientoRepository
{
    void Crear(CrearMantenimientoComando comando);
    void Eliminar(int id);
    List<MantenimientoDto> ObtenerTodos();
}