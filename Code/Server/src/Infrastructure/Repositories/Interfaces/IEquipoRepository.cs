public interface IEquipoRepository
{
    void Crear(CrearEquipoComando comando);
    void Actualizar(ActualizarEquipoComando comando);
    void Eliminar(int id);
    List<EquipoDto> ObtenerTodos();
}