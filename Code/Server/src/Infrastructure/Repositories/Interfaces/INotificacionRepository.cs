namespace IMT_Reservas.Server.Infrastructure.Repositories;

public interface INotificacionRepository
{
	Task<bool> ExisteActivoPorId(int id);
}
