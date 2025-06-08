public interface IAccesorioRepository
{
    void Crear(CrearAccesorioComando comando);
    List<AccesorioDto> ObtenerTodos();
    void Actualizar(ActualizarAccesorioComando comando);
    void Eliminar(int id);
}