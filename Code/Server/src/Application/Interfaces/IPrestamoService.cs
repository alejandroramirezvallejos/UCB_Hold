public interface IPrestamoService
{
    void CrearPrestamo(CrearPrestamoComando comando);
    List<PrestamoDto>? ObtenerTodosPrestamos();
    void EliminarPrestamo(EliminarPrestamoComando comando);
}
