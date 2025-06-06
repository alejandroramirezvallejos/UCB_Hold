//implementar
public interface IMantenimientoRepository
{
    MantenimientoDto Crear(CrearMantenimientoComando comando);
    MantenimientoDto? ObtenerPorId(int id);
    bool Eliminar(int id);
    List<MantenimientoDto> ObtenerTodos();
}