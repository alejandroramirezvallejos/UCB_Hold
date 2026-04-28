public interface IObtenerPorIdRepository<TId, TMarker, TResult>
{
    TResult ObtenerPorId(TId id);
}
