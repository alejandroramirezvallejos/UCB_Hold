using IMT_Reservas.Server.Shared.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPrestamoService
{
    void CrearPrestamo(CrearPrestamoComando comando);
    void EliminarPrestamo(EliminarPrestamoComando comando);
    List<PrestamoDto>? ObtenerTodosPrestamos();
    void AceptarPrestamo(AceptarPrestamoComando comando);
}
