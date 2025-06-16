public interface IComponenteService
{
    void CrearComponente(CrearComponenteComando comando);
    List<ComponenteDto>? ObtenerTodosComponentes();
    void ActualizarComponente(ActualizarComponenteComando comando);
    void EliminarComponente(EliminarComponenteComando comando);
}
