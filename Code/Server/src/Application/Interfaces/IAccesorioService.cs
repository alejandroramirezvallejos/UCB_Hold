public interface IAccesorioService
{
    void CrearAccesorio(CrearAccesorioComando comando);
    List<AccesorioDto>? ObtenerTodosAccesorios();
    void ActualizarAccesorio(ActualizarAccesorioComando comando);
    void EliminarAccesorio(EliminarAccesorioComando comando);
}
