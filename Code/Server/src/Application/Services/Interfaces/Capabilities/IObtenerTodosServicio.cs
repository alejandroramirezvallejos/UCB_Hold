using System.Collections.Generic;

public interface IObtenerTodosServicio<TResult>
{
    List<TResult>? ObtenerTodos();
}
