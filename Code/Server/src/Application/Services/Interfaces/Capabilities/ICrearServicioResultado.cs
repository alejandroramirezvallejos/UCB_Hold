public interface ICrearServicioResultado<TCreate, TResult>
{
    TResult Crear(TCreate comando);
}
