//implementar
public interface IComponenteRepository
{
    ComponenteDto Crear(CrearComponenteComando comando);
    ComponenteDto? ObtenerPorId(int id);
    ComponenteDto? Actualizar(ActualizarComponenteComando comando);
    bool Eliminar(int id);
    List<ComponenteDto> ObtenerTodos();
}