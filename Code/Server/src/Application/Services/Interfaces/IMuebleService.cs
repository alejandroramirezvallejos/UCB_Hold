public interface IMuebleService
{
    void CrearMueble(CrearMuebleComando comando);
    void ActualizarMueble(ActualizarMuebleComando comando);
    void EliminarMueble(EliminarMuebleComando comando);
    List<MuebleDto>? ObtenerTodosMuebles();
}
