public interface IAccesorioRepository
{
    AccesorioDto Crear(CrearAccesorioComando comando);
    AccesorioDto? ObtenerPorId(int id);
    List<AccesorioDto> ObtenerTodos();
    AccesorioDto? Actualizar(ActualizarAccesorioComando comando);
    bool Eliminar(int id);
}