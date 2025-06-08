//implementar
public interface IComponenteRepository
{
    void Crear(CrearComponenteComando comando);
    void Actualizar(ActualizarComponenteComando comando);
    void Eliminar(int id);
    List<ComponenteDto> ObtenerTodos();
}