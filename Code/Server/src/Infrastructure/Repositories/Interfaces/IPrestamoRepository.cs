public interface IPrestamoRepository
{
    void Crear(CrearPrestamoComando comando);
    void Eliminar(int id);
    List<PrestamoDto> ObtenerTodos();
}