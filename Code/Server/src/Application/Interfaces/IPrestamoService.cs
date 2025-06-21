public interface IPrestamoService
{
    void CrearPrestamo(CrearPrestamoComando comando);
    List<PrestamoDto>? ObtenerTodosPrestamos();
    List<PrestamoDto>? ObtenerPrestamosPorCarnetYEstadoPrestamo(string carnetUsuario, string estadoPrestamo);
    void EliminarPrestamo(EliminarPrestamoComando comando);
    void ActualizarEstadoPrestamo(ActualizarEstadoPrestamoComando comando);
}
