using IMT_Reservas.Server.Shared.Common;

public interface IPrestamoService
{
    void CrearPrestamo(CrearPrestamoComando comando);
    void EliminarPrestamo(EliminarPrestamoComando comando);
    List<PrestamoDto>? ObtenerTodosPrestamos();
    Task AceptarPrestamo(AceptarPrestamoComando comando);
}
