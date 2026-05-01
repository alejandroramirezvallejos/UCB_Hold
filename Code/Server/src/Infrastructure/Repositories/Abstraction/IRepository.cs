using Ardalis.Result;

namespace IMT_Reservas.Server.Infrastructure.Repositories.Abstraction;

public interface IRepository<T> where T : class
{
    Result<T?> Crear(dynamic comando);
    Result<T?> Actualizar(dynamic comando);
    Result<T?> Eliminar(dynamic comando);
    Result<List<T?>> ObtenerTodos();
}
