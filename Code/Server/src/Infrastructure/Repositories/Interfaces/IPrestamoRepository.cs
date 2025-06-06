public interface IPrestamoRepository
{
    PrestamoDto Crear(CrearPrestamoComando comando);
    PrestamoDto? ObtenerPorId(int id);
    bool Eliminar(int id);
    List<PrestamoDto> ObtenerTodos();
}